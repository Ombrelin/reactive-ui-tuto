using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.ReactiveUI;
using ReactiveUI;
using ReactiveUIExample.ViewModels;

namespace ReactiveUIExample.Views
{
    public class NugetDetailsView : ReactiveUserControl<NugetDetailsViewModel>
    {
        public static readonly RoutedEvent<TestEventArgs> TestEvent =
            RoutedEvent.Register<NugetDetailsView, TestEventArgs>(nameof(Test), RoutingStrategies.Bubble);

        public event EventHandler<TestEventArgs> Test
        {
            add => AddHandler(TestEvent, value);
            remove => RemoveHandler(TestEvent, value);
        }

        public class TestEventArgs : RoutedEventArgs
        {
            public string Data { get; set; }
        }
        
        public Image IconImage => this.FindControl<Image>("iconImage");
        public TextBlock Title => this.FindControl<TextBlock>("title");
        public TextBlock Description => this.FindControl<TextBlock>("description");
        public Button OpenButton => this.FindControl<Button>("openButton");

        public NugetDetailsView()
        {
            InitializeComponent();
            this.WhenActivated(disposableRegistration =>
            {
                ViewModel.OpenPageCommand.Subscribe(x =>
                {
                    Console.WriteLine("Open Command fired");
                    var eventArgs = new TestEventArgs {RoutedEvent = TestEvent, Data = ViewModel.Title};
                    this.RaiseEvent(eventArgs);
                });
                
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.IconUrl,
                        view => view.IconImage.Source,
                        url => url == null ? null : new Bitmap(url.ToString()))
                    .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Title,
                        view => view.Title.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Description,
                        view => view.Description.Text)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                        viewModel => viewModel.OpenPageCommand,
                        view => view.OpenButton)
                    .DisposeWith(disposableRegistration);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}