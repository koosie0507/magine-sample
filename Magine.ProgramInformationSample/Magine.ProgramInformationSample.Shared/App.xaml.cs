using System;
using System.Collections.Generic;
using System.Diagnostics;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

using Magine.ProgramInformationSample.Core;
using Magine.ProgramInformationSample.Core.ViewModel;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Magine.ProgramInformationSample
{
    /// <summary>
    ///     Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : IRouter
    {
        private readonly Dictionary<Type, ViewInfo> viewModelViewDictionary;

        private Frame rootFrame;

#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        /// <summary>
        ///     Initializes the singleton application object.  This is the first line of authored code
        ///     executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            var client = new MagineApi();
            viewModelViewDictionary = new Dictionary<Type, ViewInfo>
                                          {
                                              {   typeof(LoginViewModel), new ViewInfo
                                                  {
                                                      ViewModelFactory = () => new LoginViewModel(client, this),
                                                      ViewType = typeof(LoginPage)
                                                  }
                                              },
                                              {
                                                  typeof(ProgramInformationViewModel), new ViewInfo
                                                  {
                                                      ViewModelFactory = () => new ProgramInformationViewModel(client, this),
                                                      ViewType = typeof(MainPage)
                                                  }
                                              }
                                          };
            InitializeComponent();
            Suspending += OnSuspending;
        }

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used when the application is launched to open a specific file, to display
        ///     search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
    // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                GoTo<LoginViewModel>();
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

#if WINDOWS_PHONE_APP
    /// <summary>
    /// Restores the content transitions after the app has launched.
    /// </summary>
    /// <param name="sender">The object where the handler is attached.</param>
    /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        ///     Invoked when application execution is being suspended.  Application state is saved
        ///     without knowing whether the application will be terminated or resumed with the contents
        ///     of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        public void GoTo<T>()
        {
            if (rootFrame == null)
            {
                throw new InvalidOperationException();
            }
            try
            {
                ViewInfo info = viewModelViewDictionary[typeof(T)];
                if (!rootFrame.Navigate(info.ViewType, info.ViewModelFactory()))
                {
                    throw new Exception(
                        string.Format(
                            "Unable to navigate to view {0}, corresponding to viewmodel type \"{1}\"",
                            info.ViewType,
                            typeof(T)));
                }
            }
            catch (KeyNotFoundException e)
            {
                throw new ViewNotFoundException(e);
            }
        }

        private struct ViewInfo
        {
            internal Type ViewType { get; set; }

            internal Func<object> ViewModelFactory { get; set; }
        }
    }
}