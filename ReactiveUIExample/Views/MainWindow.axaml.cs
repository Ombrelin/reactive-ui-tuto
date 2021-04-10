using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using ReactiveUIExample.ViewModels;

namespace ReactiveUIExample.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public ListBox SearchResultsListBox => this.FindControl<ListBox>("searchResultsListBox");
        public TextBox SearchTextBox => this.FindControl<TextBox>("searchTextBox");

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            ViewModel = new MainWindowViewModel();
            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.IsAvailable,
                        view => view.SearchResultsListBox.IsVisible)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        viewModel => viewModel.SearchResults,
                        view => view.SearchResultsListBox.Items)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    vm => vm.SelectedNugetPackage,
                    v => v.SearchResultsListBox.SelectedItem
                ).DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                        viewModel => viewModel.SearchTerm,
                        view => view.SearchTextBox.Text)
                    .DisposeWith(disposableRegistration);
                
                this.SearchResultsListBox.AddHandler(NugetDetailsView.TestEvent, (sender, args) =>
                {
                    Console.WriteLine($"Event Handled for : {args.Data}");
                });
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}