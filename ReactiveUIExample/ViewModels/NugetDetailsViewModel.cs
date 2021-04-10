using System;
using System.Diagnostics;
using System.Reactive;
using NuGet.Protocol.Core.Types;
using ReactiveUI;

namespace ReactiveUIExample.ViewModels
{
    public class NugetDetailsViewModel : ViewModelBase
    {
        private readonly IPackageSearchMetadata metadata;
        private readonly Uri url;

        public NugetDetailsViewModel(IPackageSearchMetadata metadata): base()
        {
            this.metadata = metadata;
            url = new Uri("https://git.io/fAlfh");

            OpenPageCommand = ReactiveCommand.Create(OpenPage);
        }

        public ReactiveCommand<Unit, Unit> OpenPageCommand { get; }
        public Uri IconUrl => metadata.IconUrl ?? url;
        public string Description => metadata.Description;
        public Uri ProjectUrl => metadata.ProjectUrl;
        public string Title => metadata.Title;

        public void OpenPage()
        {
            Process.Start(new ProcessStartInfo(this.ProjectUrl.ToString())
            {
                UseShellExecute = true
            });
        }
    }
}