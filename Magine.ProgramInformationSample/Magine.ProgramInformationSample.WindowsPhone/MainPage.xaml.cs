using System;

using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Navigation;

using Magine.ProgramInformationSample.Core.ViewModel;

namespace Magine.ProgramInformationSample
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!(e.Parameter is ProgramInformationViewModel))
            {
                return;
            }

            var viewModel = (ProgramInformationViewModel)e.Parameter;
            StatusBar statusBar = StatusBar.GetForCurrentView();
            try
            {
                await statusBar.ProgressIndicator.ShowAsync();
                DataContext = viewModel;
                await viewModel.LoadAiringsAsync();
            }
            finally
            {
                statusBar.ProgressIndicator.HideAsync();
            }
        }
    }
}