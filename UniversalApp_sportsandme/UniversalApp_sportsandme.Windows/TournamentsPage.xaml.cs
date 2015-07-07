using UniversalApp_sportsandme.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UniversalApp_sportsandme.DataModel;
using System.Collections.ObjectModel;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace UniversalApp_sportsandme
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TournamentsPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();


        public TournamentsPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            try
            {
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Visible;
                synchProgressRing.IsActive = true;
                var TournamentsData = await DataSource.GetTournamentsAsync();
                this.DefaultViewModel["Tournaments"] = TournamentsData;
                this.DefaultViewModel["userlogin"] = DataSource.login;
            }
            catch(Exception ex)
            {
                var dialog = new MessageDialog("Ошибка при загрузке данных, повторите позже или обратитесь к администратору");
                dialog.ShowAsync();
                Frame.Navigate(typeof(AuthorizationPage));
            }
            finally
            {
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                synchProgressRing.IsActive = false;
                //CommandBarApp.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void TournamentSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((Tournament)e.ClickedItem).id;
            if(!Frame.Navigate(typeof(RoundPage), itemId))
            {
                throw new Exception("NavigationFailedExceptionMessage");
            }
        }
        private async void SynchAppButton_Click(object sender, RoutedEventArgs e)
        {
            SynchAppButton.IsEnabled = false;
            try
            {
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Visible;
                synchProgressRing.IsActive = true;
                bool saveLocalDataOnServer = true;
                await DataSource.Synch(saveLocalDataOnServer);
                var dialog = new MessageDialog("Синхронизация прошла успешно");
                dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog("В процессе синхронизации произошла ошибка.");
                dialog.ShowAsync();
            }
            finally
            {
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                synchProgressRing.IsActive = false;
                SynchAppButton.IsEnabled = true;
            }
        }
        private async void SynchWithoutSavingLocalDataAppButton_Click(object sender, RoutedEventArgs e)
        {
            SynchAppButton.IsEnabled = false;
            try
            {
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Visible;
                synchProgressRing.IsActive = true;
                bool dontSaveLocalDataOnServer = false;
                await DataSource.Synch(dontSaveLocalDataOnServer);
                var dialog = new MessageDialog("Синхронизация прошла успешно");
                dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog("В процессе синхронизации произошла ошибка.");
                try { DataSource.RefreshAccessToken(); }
                catch (Exception ex2) { Frame.Navigate(typeof(AuthorizationPage)); dialog = new MessageDialog("Ошибка авторизации"); }
                dialog.ShowAsync();
            }
            finally
            {
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                synchProgressRing.IsActive = false;
                SynchAppButton.IsEnabled = true;
            }
        }

        private async void LogoutAppButton_Click(object sender, RoutedEventArgs e)
        {
            DataSource.logout();
            await DataSource.ClearSave();
            Frame.Navigate(typeof(AuthorizationPage));
        }

    }
}
