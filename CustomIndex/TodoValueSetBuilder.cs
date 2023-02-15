using Examine;
using Umbraco.Cms.Infrastructure.Examine;

namespace UmbracoSearchingIndexing.CustomIndex
{
    public class TodoValueSetBuilder : IValueSetBuilder<ToDoModel>
    {
        public IEnumerable<ValueSet> GetValueSets(params ToDoModel[] data)
        {
            foreach (var todo in data)
            {
                var indexValues = new Dictionary<string, object>
                {
                    ["userId"] = todo.UserId,
                    ["id"] = todo.Id,
                    ["title"] = todo.Title,
                    ["completed"] = todo.Completed
                };
                var valueSet = new ValueSet(todo.Id.ToString(), "todo", indexValues);
                yield return valueSet;
            }
        }
    }
}