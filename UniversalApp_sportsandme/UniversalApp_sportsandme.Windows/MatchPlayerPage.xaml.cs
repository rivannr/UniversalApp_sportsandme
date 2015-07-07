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
using System.Text.RegularExpressions;
using Windows.UI.Popups;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace UniversalApp_sportsandme
{
    public class DataGroupMatchPlayers : INotifyPropertyChanged
    {
        private FootballMatch _match;
        private FootballTeam _team;
        private Player _player;
        private MatchPlayer _matchPlayer;
        private Boolean _showProperties;

        public event PropertyChangedEventHandler PropertyChanged;

        public FootballMatch match
        {
            get { return this._match; }
            set
            {
                if (value != this._match)
                {
                    this._match = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public Boolean showProperties
        {
            get { return this._showProperties; }
            set
            {
                if (value != this._showProperties)
                {
                    this._showProperties = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public FootballTeam team
        {
            get { return this._team; }
            set
            {
                if (value != this._team)
                {
                    this._team = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public Player player
        {
            get { return this._player; }
            set
            {
                if (value != this._player)
                {
                    this._player = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public MatchPlayer matchPlayer
        {
            get { return this._matchPlayer; }
            set
            {
                if (value != this._matchPlayer)
                {
                    this._matchPlayer = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool firstTeam { get; set; }
        public DataGroupMatchPlayers(FootballMatch match, FootballTeam team, Player player)
        {
            this.match = match;
            this.team = team;
            this.player = player;
            this.matchPlayer = new MatchPlayer(-1, -1, -1, -1);
            this.showProperties = false;

        }

        public string IsCapitan
        {
            get
            {
                return this.matchPlayer.Is_capitanValue ? "Капитан команды" : "Член команды";
            }
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MatchPlayerPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ObservableCollection<DataGroupMatchPlayers> dataGroupsMatchPlayerTeam1;
        private ObservableCollection<DataGroupMatchPlayers> dataGroupsMatchPlayerTeam2;
        private DataGroupMatchPlayers ActiveDataGroupMatchPlayersItem;
        private int activId;

        public MatchPlayerPage()
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
            //try
            //DeleteButtonEd.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            activId = (int)e.NavigationParameter;
            FootballMatch Data = await DataSource.GetItemAsync((int)e.NavigationParameter);
            dataGroupsMatchPlayerTeam1 = new ObservableCollection<DataGroupMatchPlayers>();
            dataGroupsMatchPlayerTeam2 = new ObservableCollection<DataGroupMatchPlayers>();
            DataGroupMatchPlayers dp = null;
            loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            foreach (Player pl in Data.team1_players)
            {
                dp = new DataGroupMatchPlayers(Data, Data.team1, pl);
                foreach (MatchPlayer mp in Data.matchPlayers)
                {
                    if (dp.player.id == mp.Player_idValue)
                    {
                        dp.matchPlayer = mp;
                        if (mp.objectStatus != (int)DataSource.status.Deleted && mp.objectStatus != (int)DataSource.status.needDelete) dp.showProperties = true;
                    }
                }
                if (dp.matchPlayer.IdValue == -1)
                {
                    dp.matchPlayer.Match_idValue = Data.id;
                    dp.matchPlayer.Player_idValue = pl.id;
                    dp.matchPlayer.Team_idValue = Data.team1.id;
                    dp.matchPlayer.TeamsheetValue = 0;
                }
                dataGroupsMatchPlayerTeam1.Add(dp);
            }
            foreach (Player pl in Data.team2_players)
            {
                dp = new DataGroupMatchPlayers(Data, Data.team2, pl);
                foreach (MatchPlayer mp in Data.matchPlayers)
                {
                    if (dp.player.id == mp.Player_idValue)
                    {
                        dp.matchPlayer = mp;
                        if (mp.objectStatus != (int)DataSource.status.Deleted && mp.objectStatus != (int)DataSource.status.needDelete) dp.showProperties = true;
                    }
                }
                if (dp.matchPlayer.IdValue == -1)
                {
                    dp.matchPlayer.Match_idValue = Data.id;
                    dp.matchPlayer.Player_idValue = pl.id;
                    dp.matchPlayer.Team_idValue = Data.team2.id;
                    dp.matchPlayer.TeamsheetValue = 0;
                }
                dataGroupsMatchPlayerTeam2.Add(dp);
            }

            this.DefaultViewModel["Match"] = Data;
            if (dataGroupsMatchPlayerTeam1.Count() > 0)
            {
                this.DefaultViewModel["Team1MatchPlayers"] = dataGroupsMatchPlayerTeam1;
            }
            if (dataGroupsMatchPlayerTeam2.Count() > 0)
            {
                this.DefaultViewModel["Team2MatchPlayers"] = dataGroupsMatchPlayerTeam2;
            }
            TeamComboBox.SelectedIndex = 0;

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

        private void TeamComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TeamComboBox.SelectedIndex == 0)
            {
                if (this.DefaultViewModel.ContainsKey("Team1MatchPlayers")) mainListView.ItemsSource = this.DefaultViewModel["Team1MatchPlayers"];
            }
            else
            {
                if (this.DefaultViewModel.ContainsKey("Team2MatchPlayers")) mainListView.ItemsSource = this.DefaultViewModel["Team2MatchPlayers"];
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ItemPage), activId);
        }











        private void BackSign_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(ItemPage), activId);
        }

        private async void IsCaptainRB_Unchecked(object sender, RoutedEventArgs e)
        {

            //MessageDialog md = new MessageDialog(s);
            // await md.ShowAsync();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Visible;
            synchProgressRing.IsActive = true;
            try
            {
                foreach (DataGroupMatchPlayers dgp in dataGroupsMatchPlayerTeam1)
                {
                    if (!dgp.showProperties)
                    {
                        if (dgp.matchPlayer != null)
                        {

                            if (dgp.matchPlayer.IdValue != -1)
                            {
                                if (dgp.matchPlayer.objectStatus == (int)DataSource.status.ok || dgp.matchPlayer.objectStatus == (int)DataSource.status.needUpdate)
                                {
                                    dgp.matchPlayer.objectStatus = (int)DataSource.status.needDelete;
                                    await dgp.matchPlayer.Update();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dgp.matchPlayer.IdValue == -1)
                        {
                            dgp.matchPlayer.objectStatus = (int)DataSource.status.needCreate;
                            await dgp.matchPlayer.Update();
                        }
                        else
                        {
                            dgp.matchPlayer.objectStatus = (int)DataSource.status.needUpdate;
                            await dgp.matchPlayer.Update();
                        }
                    }
                }
                foreach (DataGroupMatchPlayers dgp in dataGroupsMatchPlayerTeam2)
                {
                    if (!dgp.showProperties)
                    {
                        if (dgp.matchPlayer != null)
                        {

                            if (dgp.matchPlayer.IdValue != -1)
                            {
                                if (dgp.matchPlayer.objectStatus == (int)DataSource.status.ok || dgp.matchPlayer.objectStatus == (int)DataSource.status.needUpdate)
                                {
                                    dgp.matchPlayer.objectStatus = (int)DataSource.status.needDelete;
                                    await dgp.matchPlayer.Update();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dgp.matchPlayer.IdValue == -1)
                        {
                            dgp.matchPlayer.objectStatus = (int)DataSource.status.needCreate;
                            await dgp.matchPlayer.Update();
                        }
                        else
                        {
                            dgp.matchPlayer.objectStatus = (int)DataSource.status.needUpdate;
                            await dgp.matchPlayer.Update();
                        }
                    }
                }
                var dialog = new MessageDialog("Успешно сохранено на сервере.");
                await DataSource.Save();
                dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog("Не удалось сохранить данные на сервере.");
                dialog.ShowAsync();
            }
            finally
            {
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                synchProgressRing.IsActive = false;
            }
        }
    }
    public class IsCaptainBoolToString : IValueConverter
    {

        #region IValueConverter Members


        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            bool b = (bool)value;
            return b ? "Капитан команды" : "Член команды";
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class IsGoalkeeperBoolToString : IValueConverter
    {

        #region IValueConverter Members


        public object Convert(object value, Type targetType,
            object parameter, string language)
        {

            bool b = (bool)value;
            return b ? "вратарь" : "";
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class TeamsheetForPos1ToBool : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType,
            object parameter, string language)
        {

            int i = (int)value;
            if (i == 1)
            {
                return true;
            }
            else return false;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            if ((bool)value) return 1;
            else return -1;
        }

        #endregion
    }
    public class TeamsheetForPos2ToBool : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType,
            object parameter, string language)
        {

            int i = (int)value;
            if (i == 2)
            {
                return true;
            }
            else return false;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            if ((bool)value) return 2;
            else return -1;
        }

        #endregion
    }
    public class TeamsheetForPos3ToBool : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType,
            object parameter, string language)
        {

            int i = (int)value;
            if (i == 4)
            {
                return true;
            }
            else return false;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            if ((bool)value) return 4;
            else return -1;
        }

        #endregion
    }
    public class BoolToVisibleProp : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType,
            object parameter, string language)
        {

            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
