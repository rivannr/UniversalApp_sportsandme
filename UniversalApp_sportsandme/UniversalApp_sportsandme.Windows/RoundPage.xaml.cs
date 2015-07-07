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
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace UniversalApp_sportsandme
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RoundPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        //private int 

        public RoundPage()
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
                int tournamentId = (int)e.NavigationParameter;
                var TournamentData = await DataSource.GetTournamentAsync(tournamentId);
                this.DefaultViewModel["Tournament"] = TournamentData;
                this.DefaultViewModel["userlogin"] = DataSource.login;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                synchProgressRing.IsActive = false;
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

        private void RoundSelection_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(HubPage), ((Round)e.ClickedItem).id);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TournamentsPage));
        }
        private async void SynchAppButton_Click(object sender, RoutedEventArgs e)
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
        private void HomeAppButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TournamentsPage));
        }
    }
    public class ToUpperConv : IValueConverter
    {

        #region IValueConverter Members


        public object Convert(object value, Type targetType,
            object parameter, string language)
        {

            String s = value.ToString();
            return s.ToUpper();
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
