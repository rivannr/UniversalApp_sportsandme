using UniversalApp_sportsandme.Common;
using UniversalApp_sportsandme.DataModel;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using System.Net.Http;

// The Universal Hub Application project template is documented at http://go.microsoft.com/fwlink/?LinkID=391955

namespace UniversalApp_sportsandme
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        //private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private int PrevItemId;

        public HubPage()
        {
            this.InitializeComponent();

            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

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
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            
            try
            {
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Visible;
                synchProgressRing.IsActive = true;
                var Data = await DataSource.GetItemsAsync((int)e.NavigationParameter);
                Round RoundData = await DataSource.GetRoundAsync((int)e.NavigationParameter);
                this.DefaultViewModel["ActualMatches"] = Data.Where((status) => status.actual.Equals(true));
                this.DefaultViewModel["PastMatches"] = Data.Where((status) => status.actual.Equals(false));
                this.DefaultViewModel["RoundInfo"] = RoundData;
                this.DefaultViewModel["userlogin"] = DataSource.login;
                PrevItemId = ((Round)this.DefaultViewModel["RoundInfo"]).tournament_id;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                synchProgressRing.IsActive = false;
            }
            //var roundMatches = _DataSource.Tournaments.SelectMany(tournament => tournament.RoundsList).Where((round) => round.id.Equals(uniqueId));
            //var matches = roundMatches.SelectMany(round => round.Items);//_DataSource.Tournaments.SelectMany(tournament => tournament.RoundsList).SelectMany(round => round.Items);
            //if (matches.Count() > 0) return matches;
            
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
        //    var DataGroups = await DataSource.GetGroupsAsync();
       //     this.DefaultViewModel["Groups"] = DataGroups;
        //    if (GeneralApplicationData.accessToken == null)
       //     {
        //        loginButton.Content = "Войти";
       //        // loginButton.Click += LoginButton_Click;
        //    }
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
            // TODO: Save the unique state of the page here.
        }

        /// <summary>
        /// Shows the details of a clicked group in the <see cref="SectionPage"/>.
        /// </summary>
        /// <param name="sender">The source of the click event.</param>
        /// <param name="e">Details about the click event.</param>
        private void GroupSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            //var groupId = ((SampleDataGroup)e.ClickedItem).UniqueId;
            //if (!Frame.Navigate(typeof(SectionPage), groupId))
            //{
            //    throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            //}
            var itemId = ((FootballMatch)e.ClickedItem).id;
            if (!Frame.Navigate(typeof(ItemPage), itemId))
            {
                throw new Exception("NavigationFailedExceptionMessage");
            }
        }

        /// <summary>
        /// Shows the details of an item clicked on in the <see cref="ItemPage"/>
        /// </summary>
        /// <param name="sender">The source of the click event.</param>
        /// <param name="e">Defaults about the click event.</param>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((FootballMatch)e.ClickedItem).id;
            if (!Frame.Navigate(typeof(ItemPage), itemId))
            {
                throw new Exception("NavigationFailedExceptionMessage");
            }
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
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        

        private void backButton_Click(object sender, RoutedEventArgs e)
        {

            Frame.Navigate(typeof(RoundPage), PrevItemId);
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
    public class IsGoalToStringWithLeadZero : IValueConverter
    {

        #region IValueConverter Members


        public object Convert(object value, Type targetType,
            object parameter, string language)
        {

            int i = (int)value;
            i = 5;
            if (i < 10)
            {
                return "0" + i.ToString();
            }
            else return i.ToString();
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}