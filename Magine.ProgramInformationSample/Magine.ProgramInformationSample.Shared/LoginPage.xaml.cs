using System.ComponentModel;
using System.Linq;
using System.Net;

using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using Magine.ProgramInformationSample.Core.ViewModel;

namespace Magine.ProgramInformationSample
{
    public sealed partial class LoginPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter == null || (!(e.Parameter is LoginViewModel)))
            {
                return;
            }

            var viewModel = (LoginViewModel)e.Parameter;
            viewModel.ErrorInfo.ErrorsChanged += OnLoginErrorsChanged;
            DataContext = viewModel;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ((LoginViewModel)DataContext).ErrorInfo.ErrorsChanged -= OnLoginErrorsChanged;
            base.OnNavigatedFrom(e);
        }

        private void OnLoginErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            if (!"LoginError".Equals(e.PropertyName))
            {
                return;
            }
            var errorInfo = (INotifyDataErrorInfo)sender;
            if (!errorInfo.HasErrors)
            {
                ErrorLabel.Visibility = Visibility.Collapsed;
                ErrorLabel.ClearValue(TextBlock.TextProperty);
                return;
            }

            ErrorLabel.Visibility = Visibility.Visible;
            string error = ((INotifyDataErrorInfo)sender).GetErrors(string.Empty).OfType<string>().First();
            ErrorLabel.Text = error;
        }

        private async void OnLoginButtonClicked(object sender, RoutedEventArgs e)
        {
            var viewModel = (LoginViewModel)DataContext;
            string userName = EmailTextBox.Text;
            string password = PasswordTextBox.Password;
            EmailTextBox.ClearValue(TextBox.TextProperty);
            PasswordTextBox.ClearValue(PasswordBox.PasswordProperty);
            try
            {
                ProgressBar.Visibility = Visibility.Visible;
                await viewModel.Login(new NetworkCredential(userName, password));
            }
            finally
            {
                ProgressBar.Visibility = Visibility.Collapsed;
            }
        }
    }
}