using Examine;
using Umbraco.Cms.Core.Composing;
using UmbracoSearchingIndexing.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.Examine;
using UmbracoSearchingIndexing.Components;
using UmbracoExamine.PDF;
using UmbracoSearchingIndexing.CustomIndex;

namespace UmbracoSearchingIndexing.Composers
{
    [ComposeAfter(typeof(ExaminePdfComposer))]
    public class ExamineComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<ISearchService, SearchService>();
            builder.Components().Append<ExamineComponents>();
            builder.Services.ConfigureOptions<ConfigureCustomFieldOptions>();

            //Custom Index
            builder.Services.AddSingleton<TodoValueSetBuilder>();
            builder.Services.AddSingleton<IIndexPopulator, TodoIndexPopulator>();
            builder.Services
                .AddExamineLuceneIndex<CustomToDoIndex, ConfigurationEnabledDirectoryFactory>("TodoIndex");
        }
    }
}