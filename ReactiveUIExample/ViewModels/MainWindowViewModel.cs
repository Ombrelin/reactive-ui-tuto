using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ReactiveUIExample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";
        
        [Reactive] 
        public string SearchTerm { get; set; }

        [Reactive] 
        public NugetDetailsViewModel SelectedNugetPackage { get; set; }

        public ReactiveCommand<NugetDetailsViewModel,Unit> NugetPackageSelectedCommand { get; }
        
        private readonly ObservableAsPropertyHelper<IEnumerable<NugetDetailsViewModel>> searchResults;
        public IEnumerable<NugetDetailsViewModel> SearchResults => searchResults.Value;
        
        private readonly ObservableAsPropertyHelper<bool> isAvailable;
        public bool IsAvailable => isAvailable.Value;

        public MainWindowViewModel(): base()
        {
            searchResults = this
                .WhenAnyValue(x => x.SearchTerm)
                .Throttle(TimeSpan.FromMilliseconds(800))
                .DistinctUntilChanged()
                .Where(term => !string.IsNullOrWhiteSpace(term))
                .Select(term => term.Trim())
                .SelectMany(SearchNuggetPackages)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.SearchResults);

            isAvailable = this
                .WhenAnyValue(x => x.SearchResults)
                .Select(searchResults => this.searchResults != null)
                .ToProperty(this, x => x.IsAvailable);

            this.WhenAnyValue(x => x.SelectedNugetPackage)
                .Where(v => v != null)
                .Subscribe(v => Console.WriteLine(v.Title));

        }

        public async Task<IEnumerable<NugetDetailsViewModel>> SearchNuggetPackages(string term, CancellationToken token)
        {
            Console.WriteLine(term);
            var providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());
            var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            var source = new SourceRepository(packageSource, providers);
            
            ILogger logger = NullLogger.Instance;

            var filter = new SearchFilter(false);
            var resource = await source.GetResourceAsync<PackageSearchResource>().ConfigureAwait(false);
            var metadata = await resource.SearchAsync(term, filter, 0, 10, logger, token).ConfigureAwait(false);
            metadata.ToList().ForEach(m => Console.WriteLine(m.Title));
            return metadata.Select(x => new NugetDetailsViewModel(x));
        }
    }
}