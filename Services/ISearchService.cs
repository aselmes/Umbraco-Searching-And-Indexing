using System.Collections.Generic;
using Examine;
using UmbracoSearchingIndexing.Models;

using UmbracoSearchingIndexing.Models;

namespace UmbracoSearchingIndexing.Services
{
    public interface ISearchService
    {
        IEnumerable<ISearchResult> GetSearchResults(string searchTerm, string contentType, out long totalItemCount);

        IEnumerable<SearchResultItem> GetContentSearchResults(string searchTerm, string contentType, out long totalItemCount);
    }
}