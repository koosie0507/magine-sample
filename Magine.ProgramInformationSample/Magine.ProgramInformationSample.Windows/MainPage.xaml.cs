using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Magine.ProgramInformationSample.Core.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Magine.ProgramInformationSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter == null || (!(e.Parameter is ProgramInformationViewModel)))
            {
                return;
            }

            var viewModel = (ProgramInformationViewModel)e.Parameter;
            DataContext = viewModel;
            try
            {
                ProgressRing.IsActive = true;
                await viewModel.LoadAiringsAsync();
            }
            finally
            {
                ProgressRing.IsActive = false;
            }
        }
    }
}
