using System.Collections.Generic;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbracoSearchingIndexing.Models
{
    public class SearchModel : PublishedContentWrapped
    {
        public SearchModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
        {
        }

        public string Query { get; set; }
        public long TotalResults { get; set; }
        public IEnumerable<SearchResultItem> SearchResults { get; set; }
        public string DoctypeToSearch { get; set; }
    }
}