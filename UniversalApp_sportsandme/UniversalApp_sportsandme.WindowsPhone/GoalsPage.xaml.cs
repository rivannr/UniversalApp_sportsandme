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
    public class DataGroupGoalsPlayers : INotifyPropertyChanged
    {
        private FootballMatch _match;
        private FootballTeam _team;
        private Player _player;
        private Goal _goals;
        private string _assName;

        public event PropertyChangedEventHandler PropertyChanged;
        public string assName
        {
            get { return this._assName; }
            set
            {
                if (value != this._assName)
                {
                    this._assName = value;
                    NotifyPropertyChanged();
                }
            }
        }

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
        public Goal goal
        {
            get { return this._goals; }
            set
            {
                if (value != this._goals)
                {
                    this._goals = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool firstTeam { get; set; }
        public DataGroupGoalsPlayers(FootballMatch match, FootballTeam team, Player player, Goal goal)
        {
            this.match = match;
            this.team = team;
            this.player = player;
            this._goals = goal;
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
    public sealed partial class GoalsPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ObservableCollection<DataGroupGoalsPlayers> dataGroupsGoalsPlayer;
        private DataGroupGoalsPlayers ActiveDataGroupGoalItem;

        private int activId;

        public GoalsPage()
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
            LoadState();
            TeamComboBox.SelectedIndex = 0;
        }
        private async void LoadState()
        {
            DeleteButtonEd.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            FootballMatch Data = await DataSource.GetItemAsync(activId);
            dataGroupsGoalsPlayer = new ObservableCollection<DataGroupGoalsPlayers>();
            foreach (Goal mp in Data.goals)
            {
                DataGroupGoalsPlayers dp = null;
                if (mp.objectStatus != (int)DataSource.status.Deleted || mp.objectStatus != (int)DataSource.status.needDelete)
                {
                    if (mp.Team_idValue == Data.team1.id)
                    {
                        var matches = Data.team1_players.Where((player) => player.id.Equals(mp.Player_idValue));
                        if (matches.Count() > 0)
                        {
                            dp = new DataGroupGoalsPlayers(Data, Data.team1, matches.First(), mp);
                            dp.assName = "Не было помощника";
                            dp.firstTeam = true;
                            foreach (Player ass in Data.team1_players)
                            {
                                if (ass.id == dp.goal.Assistant_idValue)
                                {
                                    dp.assName = ass.full_name;
                                }
                            }
                        }
                    }
                    else
                    {
                        var matches = Data.team2_players.Where((player) => player.id.Equals(mp.Player_idValue));
                        if (matches.Count() > 0)
                        {
                            dp = new DataGroupGoalsPlayers(Data, Data.team2, matches.First(), mp);
                            dp.assName = "Не было помощника";
                            dp.firstTeam = false;
                            foreach (Player ass in Data.team2_players)
                            {
                                if (ass.id == dp.goal.Assistant_idValue)
                                {
                                    dp.assName = ass.full_name;
                                }
                            }
                        }
                    }
                    if (dp != null) dataGroupsGoalsPlayer.Add(dp);
                }
            }
            this.DefaultViewModel["Match"] = Data;
            if (dataGroupsGoalsPlayer.Count() > 0)
            {
                this.DefaultViewModel["ActivElement"] = dataGroupsGoalsPlayer.First();
                this.DefaultViewModel["dataGroupsMatchPlayer"] = dataGroupsGoalsPlayer;
                this.DefaultViewModel["Team1MatchPlayers"] = dataGroupsGoalsPlayer.Where((team) => team.team.id.Equals(Data.team1.id));
                this.DefaultViewModel["Team2MatchPlayers"] = dataGroupsGoalsPlayer.Where((team) => team.team.id.Equals(Data.team2.id));

            }
            if (TeamComboBox.SelectedIndex == 0)
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
            Player tmp = null;
            this.DefaultViewModel["ActivElement"] = (DataGroupGoalsPlayers)e.ClickedItem;
            ActiveDataGroupGoalItem = (DataGroupGoalsPlayers)e.ClickedItem;
            viewPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            editorPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            DeleteButtonEd.Visibility = Windows.UI.Xaml.Visibility.Visible;
            AcceptButtonEd.Content = "Изменить";
            editorPanel.Tag = "Edit";
            if (ActiveDataGroupGoalItem.firstTeam)
            {
                TeamComboBoxEd.SelectedIndex = 0;
            }
            else
            {
                TeamComboBoxEd.SelectedIndex = 1;
            }
            PlayerComboBoxEd.SelectedItem = ActiveDataGroupGoalItem.player;

            // = ActiveDataGroupRCPlayersItem.redCard.NoteValue;
            MinuteTextBox.Text = ActiveDataGroupGoalItem.goal.MinuteValue.ToString();
            AdditionMinuteTextBox.Text = ActiveDataGroupGoalItem.goal.Addition_minuteValue.ToString();
            if (ActiveDataGroupGoalItem.goal.Assistant_idValue != 0)
            {

                try
                {
                    tmp = (Player)AssistantComboBoxEd.SelectedItem;
                }
                catch
                {
                    tmp = null;
                }
                if (tmp != null)
                {

                }
            }
            if (ActiveDataGroupGoalItem.goal.Assistant_idValue != 0)
            {
                try
                {
                    foreach (Player cbi in AssistantComboBoxEd.Items)
                    {
                        try
                        {
                            tmp = (Player)cbi;
                        }
                        catch (Exception ex)
                        {
                            tmp = null;
                        }
                        if (tmp != null)
                        {
                            if (cbi.id == ActiveDataGroupGoalItem.goal.Assistant_idValue)
                            {
                                AssistantComboBoxEd.SelectedItem = cbi;
                            }
                        }
                    }
                }
                catch (Exception ex2)
                {

                }
            }

            IsAutogoalEd.IsChecked = ActiveDataGroupGoalItem.goal.Is_autogoalValue;
            IsPenaltyEd.IsChecked = ActiveDataGroupGoalItem.goal.Is_penaltyValue;
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
                AssistantComboBoxEd.ItemsSource = ((FootballMatch)this.DefaultViewModel["Match"]).team1_players;
            }
            else
            {
                PlayerComboBoxEd.ItemsSource = ((FootballMatch)this.DefaultViewModel["Match"]).team2_players;
                this.DefaultViewModel["AssistantsPlayers"] = ((FootballMatch)this.DefaultViewModel["Match"]).team2_players;

                AssistantComboBoxEd.ItemsSource = ((FootballMatch)this.DefaultViewModel["Match"]).team2_players;
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
                                ActiveDataGroupGoalItem.firstTeam = true;
                            }
                            else
                            {
                                ft = ((FootballMatch)this.DefaultViewModel["Match"]).team2;
                                ActiveDataGroupGoalItem.firstTeam = false;
                            }
                            ActiveDataGroupGoalItem.goal.Player_idValue = p.id;
                            ActiveDataGroupGoalItem.player = p;
                            ActiveDataGroupGoalItem.team = ft;
                            ActiveDataGroupGoalItem.goal.Team_idValue = ft.id;
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
                            ActiveDataGroupGoalItem.goal.MinuteValue = iM;
                            ActiveDataGroupGoalItem.goal.Addition_minuteValue = iAM;
                            if (AssistantComboBoxEd.SelectedIndex != -1) ActiveDataGroupGoalItem.goal.Assistant_idValue = ((Player)AssistantComboBoxEd.SelectedItem).id;
                            else ActiveDataGroupGoalItem.goal.Assistant_idValue = 0;
                            ActiveDataGroupGoalItem.goal.Is_penaltyValue = (bool)IsPenaltyEd.IsChecked;
                            ActiveDataGroupGoalItem.goal.Is_autogoalValue = (bool)IsAutogoalEd.IsChecked;
                            ActiveDataGroupGoalItem.goal.objectStatus = (int)DataSource.status.needUpdate;
                            await ActiveDataGroupGoalItem.goal.Update();


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
                            int t = 0;
                            if (AssistantComboBoxEd.SelectedIndex != -1) t = ((Player)AssistantComboBoxEd.SelectedItem).id;

                            Goal mp = new Goal(0, ((FootballMatch)this.DefaultViewModel["Match"]).id, ft.id, ((Player)PlayerComboBoxEd.SelectedItem).id, t, (bool)IsAutogoalEd.IsChecked, (bool)IsPenaltyEd.IsChecked, iM, iAM);
                            mp.objectStatus = (int)DataSource.status.needCreate;
                            await mp.Update();

                            FootballMatch fm = await DataSource.GetItemAsync(activId);
                            if (mp.objectStatus != (int)DataSource.status.Deleted || mp.objectStatus != (int)DataSource.status.needDelete)
                            {
                                DataGroupGoalsPlayers dgm = new DataGroupGoalsPlayers(fm, ft, ((Player)PlayerComboBoxEd.SelectedItem), mp);
                                if (first)
                                {
                                    dgm.assName = "Не было помощника";
                                    dgm.firstTeam = true;
                                    foreach (Player ass in ((FootballMatch)this.DefaultViewModel["Match"]).team1_players)
                                    {
                                        if (ass.id == dgm.goal.Assistant_idValue)
                                        {
                                            dgm.assName = ass.full_name;
                                        }
                                    }
                                }
                                else
                                {
                                    dgm.assName = "Не было помощника";
                                    dgm.firstTeam = false;
                                    foreach (Player ass in ((FootballMatch)this.DefaultViewModel["Match"]).team2_players)
                                    {
                                        if (ass.id == dgm.goal.Assistant_idValue)
                                        {
                                            dgm.assName = ass.full_name;
                                        }
                                    }
                                }

                                
                                //this.DefaultViewModel["Team1MatchPlayers"] = dataGroupsGoalsPlayer.Where((team) => team.team.id.Equals(fm.team1.id));
                                //this.DefaultViewModel["Team2MatchPlayers"] = dataGroupsGoalsPlayer.Where((team) => team.team.id.Equals(fm.team2.id));
                                
                            }
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
                if (TeamComboBox.SelectedIndex == 0)
                {
                    if (this.DefaultViewModel.ContainsKey("Team1MatchPlayers")) mainListView.ItemsSource = this.DefaultViewModel["Team1MatchPlayers"];
                }
                else
                {
                    if (this.DefaultViewModel.ContainsKey("Team2MatchPlayers")) mainListView.ItemsSource = this.DefaultViewModel["Team2MatchPlayers"];
                }
            }
        }

        private async void DeleteButtonEd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                synchProgressRing.IsActive = true;
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Visible;
                if (ActiveDataGroupGoalItem != null)
                {
                    
                    ((ObservableCollection<DataGroupGoalsPlayers>)this.DefaultViewModel["dataGroupsMatchPlayer"]).Remove(ActiveDataGroupGoalItem);
                    if (ActiveDataGroupGoalItem.firstTeam)
                    {
                        this.DefaultViewModel["Team1MatchPlayers"] = dataGroupsGoalsPlayer.Where((team) => team.team.id.Equals(((FootballMatch)this.DefaultViewModel["Match"]).team1.id));

                    }
                    else
                    {
                        this.DefaultViewModel["Team2MatchPlayers"] = dataGroupsGoalsPlayer.Where((team) => team.team.id.Equals(((FootballMatch)this.DefaultViewModel["Match"]).team2.id));
                    }
                    viewPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    editorPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    DeleteButtonEd.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    await ActiveDataGroupGoalItem.goal.Delete();
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
            AssistantComboBoxEd.SelectedIndex = -1;
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

        private void ClearAssistButtonEd_Click(object sender, RoutedEventArgs e)
        {
            AssistantComboBoxEd.SelectedIndex = -1;
        }
    }
    public class IsAutogoalToString : IValueConverter
    {

        #region IValueConverter Members


        public object Convert(object value, Type targetType,
            object parameter, string language)
        {

            bool b = (bool)value;
            return b ? "Автогол" : "";
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class IsPenaltyToString : IValueConverter
    {

        #region IValueConverter Members


        public object Convert(object value, Type targetType,
            object parameter, string language)
        {

            bool b = (bool)value;
            return b ? "Пенальти" : "";
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class StatusToString : IValueConverter
    {

        #region IValueConverter Members


        public object Convert(object value, Type targetType,
            object parameter, string language)
        {

            int i = (int)value;
            if (i == (int)DataSource.status.ok)
            {
                return "";
            }
            else return "Не сохранено на сервере";
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    //IsGoalToStringWithLeadZero
    

}
