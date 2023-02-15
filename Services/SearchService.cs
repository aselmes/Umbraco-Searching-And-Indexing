using Examine;
using UmbracoSearchingIndexing.Models;
using System;
using System.Collections.Generic;

using UmbracoSearchingIndexing.Models;

using UmbracoSearchingIndexing.Services;
using Umbraco.Cms.Core.Web;
using System.Globalization;
using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.Examine;
using System.Diagnostics;
using Umbraco.Cms.Core.Logging;
using Examine.Search;
using UmbracoSearchingIndexing.CustomIndex;

namespace UmbracoSearchingIndexing.Services
{
    public class SearchService : ISearchService
    {
        private readonly IExamineManager _examineManager;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IProfiler _profiler;

        public SearchService(IExamineManager examineManager, IUmbracoContextAccessor umbracoContextAccessor, IProfiler profiler)
        {
            _examineManager = examineManager;
            _umbracoContextAccessor = umbracoContextAccessor;
            _profiler = profiler;
        }

        public IEnumerable<SearchResultItem> GetContentSearchResults(string searchTerm, string contentType, out long totalItemCount)
        {
            var pageOfResults = GetSearchResults(searchTerm, contentType, out totalItemCount);
            var items = new List<SearchResultItem>();
            if (pageOfResults != null && pageOfResults.Any())
            {
                foreach (var item in pageOfResults)
                {
                    if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
                    {
                        var page = umbracoContext.Content.GetById(int.Parse(item.Id));
                        var pageMedia = umbracoContext.Media.GetById(int.Parse(item.Id));
                        if (page != null)
                        {
                            items.Add(new SearchResultItem()
                            {
                                PublishedItem = page,
                                Score = item.Score
                            });
                        }
                        if (pageMedia != null)
                        {
                            items.Add(new SearchResultItem()
                            {
                                PublishedItem = pageMedia,
                                Score = item.Score
                            });
                        }
                        if (page == null && pageMedia == null)
                        {
                            //we got no content and no media, so we assume the item is a ToDo
                            items.Add(new SearchResultItem()
                            {
                                PublishedItem = null,
                                ToDo = new ToDoModel() { Title = item.GetValues("title").FirstOrDefault(), Id = Int32.Parse(item.GetValues("id").FirstOrDefault()) },
                                Score = item.Score
                            });
                        }
                    }
                }
            }
            return items;
        }

        public IEnumerable<ISearchResult> GetSearchResults(string searchTerm, string contentType, out long totalItemCount)
        {
            totalItemCount = 0;
            if (_examineManager.TryGetSearcher("MultiSearcher", out var multiSearcher))
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return Array.Empty<ISearchResult>();
                }
                var fieldToSearchLang = "contents" + "_" + CultureInfo.CurrentCulture.ToString().ToLower();
                var fieldToSearchInvariant = "contents";
                var hideFromNavigation = "umbracoNaviHide";
                var pdfTextContent = "fileTextContent";
                var todoTitle = "title";
                var fieldsToSearch = new[] { fieldToSearchLang, fieldToSearchInvariant, pdfTextContent, todoTitle };
                var criteria = multiSearcher.CreateQuery(null, BooleanOperation.Or);
                var examineQuery = criteria.GroupedOr(fieldsToSearch, searchTerm.MultipleCharacterWildcard());
                examineQuery.Not().Field(hideFromNavigation, 1.ToString());
                //boost items with the boosted field checked
                examineQuery.Or().Field("boosted", 1.ToString().Boost(20f));
                examineQuery.OrderByDescending(new SortableField("publishingDate", SortType.Long));
                if (contentType != "All")
                {
                    examineQuery.And().Field("__NodeTypeAlias", contentType);
                }

                using (_profiler.Step("Examine query"))
                {
                    var results = examineQuery.Execute();
                    totalItemCount = results.TotalItemCount;
                    if (results.Any())
                    {
                        Debug.WriteLine(criteria.ToString());

                        return results;
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                }
            }
            return Enumerable.Empty<ISearchResult>();
        }
    }
}