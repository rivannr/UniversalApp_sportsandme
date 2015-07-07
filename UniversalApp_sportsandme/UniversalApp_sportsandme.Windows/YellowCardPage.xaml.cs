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
using System.Threading.Tasks;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace UniversalApp_sportsandme
{
    public class DataGroupYCPlayers : INotifyPropertyChanged
    {
        private FootballMatch _match;
        private FootballTeam _team;
        private Player _player;
        private YellowCard _yellowCard;

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
        public YellowCard yellowCard
        {
            get { return this._yellowCard; }
            set
            {
                if (value != this._yellowCard)
                {
                    this._yellowCard = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool firstTeam { get; set; }
        public DataGroupYCPlayers(FootballMatch match, FootballTeam team, Player player, YellowCard yellowCard)
        {
            this.match = match;
            this.team = team;
            this.player = player;
            this.yellowCard = yellowCard;
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
    public sealed partial class YellowCardPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ObservableCollection<DataGroupYCPlayers> dataGroupsYCPlayer;
        private DataGroupYCPlayers ActiveDataGroupYCPlayersItem;
        private int activId;

        public YellowCardPage()
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

            activId = (int)e.NavigationParameter;
            TeamComboBox.SelectedIndex = 0;
            await LoadState();

        }
        public async Task LoadState()
        {
            DeleteButtonEd.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            FootballMatch Data = await DataSource.GetItemAsync(activId);
            dataGroupsYCPlayer = new ObservableCollection<DataGroupYCPlayers>();
            foreach (YellowCard mp in Data.yellowcards)
            {
                DataGroupYCPlayers dp = null;
                if (mp.objectStatus != (int)DataSource.status.Deleted || mp.objectStatus != (int)DataSource.status.needDelete)
                {
                    if (mp.Team_idValue == Data.team1.id)
                    {
                        var matches = Data.team1_players.Where((player) => player.id.Equals(mp.Player_idValue));
                        if (matches.Count() > 0)
                        {
                            dp = new DataGroupYCPlayers(Data, Data.team1, matches.First(), mp);
                            dp.firstTeam = true;
                        }
                    }
                    else
                    {
                        var matches = Data.team2_players.Where((player) => player.id.Equals(mp.Player_idValue));
                        if (matches.Count() > 0)
                        {
                            dp = new DataGroupYCPlayers(Data, Data.team2, matches.First(), mp);
                            dp.firstTeam = false;
                        }
                    }
                    if (dp != null) dataGroupsYCPlayer.Add(dp);
                }
            }
            this.DefaultViewModel["Match"] = Data;
            if (dataGroupsYCPlayer.Count() > 0)
            {
                this.DefaultViewModel["ActivElement"] = dataGroupsYCPlayer.First();
                this.DefaultViewModel["dataGroupsMatchPlayer"] = dataGroupsYCPlayer;
                this.DefaultViewModel["Team1MatchPlayers"] = dataGroupsYCPlayer.Where((team) => team.team.id.Equals(Data.team1.id));
                this.DefaultViewModel["Team2MatchPlayers"] = dataGroupsYCPlayer.Where((team) => team.team.id.Equals(Data.team2.id));
            }
            if (TeamComboBoxEd.SelectedIndex == 0)
            {
                if (this.DefaultViewModel.ContainsKey("Team1MatchPlayers")) mainListView.ItemsSource = this.DefaultViewModel["Team1MatchPlayers"];
            }
            else
            {
                if (this.DefaultViewModel.ContainsKey("Team2MatchPlayers")) mainListView.ItemsSource = this.DefaultViewModel["Team2MatchPlayers"];
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
        private void GroupSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            //var groupId = ((SampleDataGroup)e.ClickedItem).UniqueId;
            //if (!Frame.Navigate(typeof(SectionPage), groupId))
            //{
            //    throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            //}
            // var itemId = ((FootballMatch)e.ClickedItem).id;
            //Frame.Navigate(typeof(ItemPage), itemId);
            this.DefaultViewModel["ActivElement"] = (DataGroupYCPlayers)e.ClickedItem;
            ActiveDataGroupYCPlayersItem = (DataGroupYCPlayers)e.ClickedItem;
            viewPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            editorPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            DeleteButtonEd.Visibility = Windows.UI.Xaml.Visibility.Visible;
            AcceptButtonEd.Content = "Изменить";
            editorPanel.Tag = "Edit";
            if (ActiveDataGroupYCPlayersItem.firstTeam)
            {
                TeamComboBoxEd.SelectedIndex = 0;
            }
            else
            {
                TeamComboBoxEd.SelectedIndex = 1;
            }
            PlayerComboBoxEd.SelectedItem = ActiveDataGroupYCPlayersItem.player;

            NoteTextBox.Text = ActiveDataGroupYCPlayersItem.yellowCard.NoteValue;
            MinuteTextBox.Text = ActiveDataGroupYCPlayersItem.yellowCard.MinuteValue.ToString();
            AdditionMinuteTextBox.Text = ActiveDataGroupYCPlayersItem.yellowCard.Addition_minuteValue.ToString();
            //if (ActiveDataGroupYCPlayersItem.matchPlayer.TeamsheetValue != 4)
            //{
            //    PositionComboBoxEd.SelectedIndex = ActiveDataGroupYCPlayersItem.matchPlayer.TeamsheetValue;
            //}
            //else
            //{
            //    PositionComboBoxEd.SelectedIndex = 3;
            //}
            //IsCapitanEd.IsChecked = ActiveDataGroupMatchPlayersItem.matchPlayer.Is_capitanValue;
            //IsGoalkeeperEd.IsChecked = ActiveDataGroupMatchPlayersItem.matchPlayer.Is_goalkeeperValue;


        }
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

        private void BackButtonEd_Click(object sender, RoutedEventArgs e)
        {
            viewPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            editorPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            DeleteButtonEd.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void TeamComboBoxEd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TeamComboBoxEd.SelectedIndex == 0)
            {
                PlayerComboBoxEd.ItemsSource = ((FootballMatch)this.DefaultViewModel["Match"]).team1_players;
            }
            else
            {
                PlayerComboBoxEd.ItemsSource = ((FootballMatch)this.DefaultViewModel["Match"]).team2_players;
            }
        }

        private async void AcceptButtonEd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                synchProgressRing.IsActive = true;
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Visible;
                if (PlayerComboBoxEd.SelectedItem != null && TeamComboBoxEd.SelectedItem != null)
                {
                    if (PlayerComboBoxEd.SelectedItem.GetType().Equals(typeof(Player)))
                    {
                        if (editorPanel.Tag.ToString() == "Edit")
                        {

                            Player p = (Player)PlayerComboBoxEd.SelectedItem;
                            FootballTeam ft;
                            if (TeamComboBoxEd.SelectedIndex == 0)
                            {
                                ft = ((FootballMatch)this.DefaultViewModel["Match"]).team1;
                                ActiveDataGroupYCPlayersItem.firstTeam = true;
                            }
                            else
                            {
                                ft = ((FootballMatch)this.DefaultViewModel["Match"]).team2;
                                ActiveDataGroupYCPlayersItem.firstTeam = false;
                            }
                            ActiveDataGroupYCPlayersItem.yellowCard.Player_idValue = p.id;
                            ActiveDataGroupYCPlayersItem.player = p;
                            ActiveDataGroupYCPlayersItem.team = ft;
                            ActiveDataGroupYCPlayersItem.yellowCard.Team_idValue = ft.id;
                            int iM = 0;
                            int iAM = 0;
                            try
                            {
                                iM = Int32.Parse(MinuteTextBox.Text);
                                iAM = Int32.Parse(AdditionMinuteTextBox.Text);
                            }
                            catch
                            {
                                iM = 0;
                                iAM = 0;
                            }
                            ActiveDataGroupYCPlayersItem.yellowCard.MinuteValue = iM;
                            ActiveDataGroupYCPlayersItem.yellowCard.Addition_minuteValue = iAM;
                            ActiveDataGroupYCPlayersItem.yellowCard.NoteValue = NoteTextBox.Text;
                            ActiveDataGroupYCPlayersItem.yellowCard.objectStatus = (int)DataSource.status.needUpdate;
                            await ActiveDataGroupYCPlayersItem.yellowCard.Update();


                        }
                        else
                        {
                            FootballTeam ft;
                            bool first;
                            if (TeamComboBoxEd.SelectedIndex == 0)
                            {
                                ft = ((FootballMatch)this.DefaultViewModel["Match"]).team1;
                                first = true;
                            }
                            else
                            {
                                ft = ((FootballMatch)this.DefaultViewModel["Match"]).team2;
                                first = false;
                            }
                            int iM = 0;
                            int iAM = 0;
                            try
                            {
                                iM = Int32.Parse(MinuteTextBox.Text);
                                iAM = Int32.Parse(AdditionMinuteTextBox.Text);
                            }
                            catch
                            {
                                if(iM == null)iM = 0;
                                if(iAM == null)iAM = 0;
                            }
                            YellowCard mp = new YellowCard(0, ((FootballMatch)this.DefaultViewModel["Match"]).id, ft.id, ((Player)PlayerComboBoxEd.SelectedItem).id, iM, iAM, NoteTextBox.Text);
                            mp.objectStatus = (int)DataSource.status.needCreate;
                            await mp.Update();

                          //  FootballMatch fm = await DataSource.GetItemAsync(activId);
                         //   if (mp.objectStatus != (int)DataSource.status.Deleted || mp.objectStatus != (int)DataSource.status.needDelete)
                         //   {
                             //   DataGroupYCPlayers dgm = new DataGroupYCPlayers(fm, ft, ((Player)PlayerComboBoxEd.SelectedItem), mp);
                             //   if (first)
                             //   {
                             //       dgm.firstTeam = true;
                             //   }
                             //   else
                             //   {
                             //       dgm.firstTeam = false;
                              //  }
                                //dataGroupsYCPlayer.Add(dgm);
                               // this.DefaultViewModel["dataGroupsMatchPlayer"] = dataGroupsYCPlayer;
                               // this.DefaultViewModel["Team1MatchPlayers"] = dataGroupsYCPlayer.Where((team) => team.team.id.Equals(fm.team1.id));
                                //this.DefaultViewModel["Team2MatchPlayers"] = dataGroupsYCPlayer.Where((team) => team.team.id.Equals(fm.team2.id));
                            //}
                            //Frame.Navigate(typeof(MatchPlayerPage), activId);
                        }
                        viewPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        editorPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        DeleteButtonEd.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
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
                LoadState();
                synchProgressRing.IsActive = false;
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private async void DeleteButtonEd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                synchProgressRing.IsActive = true;
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Visible;
                if (ActiveDataGroupYCPlayersItem != null)
                {

                    ((ObservableCollection<DataGroupYCPlayers>)this.DefaultViewModel["dataGroupsMatchPlayer"]).Remove(ActiveDataGroupYCPlayersItem);
                    if (ActiveDataGroupYCPlayersItem.firstTeam)
                    {
                        this.DefaultViewModel["Team1MatchPlayers"] = dataGroupsYCPlayer.Where((team) => team.team.id.Equals(((FootballMatch)this.DefaultViewModel["Match"]).team1.id));

                    }
                    else
                    {
                        this.DefaultViewModel["Team2MatchPlayers"] = dataGroupsYCPlayer.Where((team) => team.team.id.Equals(((FootballMatch)this.DefaultViewModel["Match"]).team2.id));
                    }
                    viewPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    editorPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    DeleteButtonEd.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    await ActiveDataGroupYCPlayersItem.yellowCard.Delete();
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
                LoadState();
                synchProgressRing.IsActive = false;
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            viewPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            editorPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            DeleteButtonEd.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            editorPanel.Tag = "Add";
            AcceptButtonEd.Content = "Добавить";
            PlayerComboBoxEd.SelectedIndex = -1;
            TeamComboBoxEd.SelectedIndex = -1;
            AdditionMinuteTextBox.Text = "";
            MinuteTextBox.Text = "";
            NoteTextBox.Text = "";
        }

        private void BackSign_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(ItemPage), activId);
        }

        private void MinuteTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");

            MinuteTextBox.Text = Regex.Replace(MinuteTextBox.Text, @"[^0-9]", "");
        }

        private void AdditionMinuteTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");

            AdditionMinuteTextBox.Text = Regex.Replace(AdditionMinuteTextBox.Text, @"[^0-9]", "");
        }
    }

}
