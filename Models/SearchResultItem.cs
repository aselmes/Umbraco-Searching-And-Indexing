using Umbraco.Cms.Core.Models.PublishedContent;
using UmbracoSearchingIndexing.CustomIndex;

namespace UmbracoSearchingIndexing.Models
{
    public class SearchResultItem
    {
        public IPublishedContent PublishedItem { get; init; }
        public ToDoModel ToDo { get; init; }
        public float Score { get; init; }
    }
}