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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace UniversalApp_sportsandme
{
    public class ConcTime : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        FootballMatch fm;
        private string _time;
        public string time
        {
            get { return _time; }
            set
            {
                string DateTime = fm.start_at;
                string _date = DateTime.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Distinct().First();
                if (value != this._time)
                {
                    this._time = value;
                    fm.start_at = _date +" "+ _time;
                    NotifyPropertyChanged();
                }
            }
        }
        public ConcTime(string DateTime, FootballMatch fm)
        {
            this.fm = fm;
            time = DateTime.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Distinct().Last();
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public sealed partial class MatchMainInfoPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private int activId;

        public MatchMainInfoPage()
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
                FootballMatch Data = await DataSource.GetItemAsync((int)e.NavigationParameter);
                
                activId = (int)e.NavigationParameter;
                if (Data != null)
                {
                    this.DefaultViewModel["MatchInfo"] = Data;
                }
                ObservableCollection<int> scoreValue = new ObservableCollection<int>();
                for (int i = 0; i <= 40; i++)
                {
                    scoreValue.Add(i);
                }
                this.DefaultViewModel["scoreValue"] = scoreValue;
                this.DefaultViewModel["_timePart"] = new ConcTime(Data.start_at, Data);
                //Goals1ComboBoxEd.ItemsSource = this.DefaultViewModel["scoreValue"];
                //Goals2ComboBoxEd.ItemsSource = this.DefaultViewModel["scoreValue"];
                //penalty1ComboBoxEd.ItemsSource = this.DefaultViewModel["scoreValue"];
                //penalty2ComboBoxEd.ItemsSource = this.DefaultViewModel["scoreValue"];
                //dpFromDate.Date = DateTime.Parse(Data.start_at);
                ///dpFromDate.
                //ObservableColletcion scoreValue = new ObservableColletcion;
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

        private async void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                synchProgressRing.IsActive = true;
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Visible;
                await ((FootballMatch)this.DefaultViewModel["MatchInfo"]).UpdateMatchInfo();
                var dialog = new MessageDialog("Успешно сохранено на сервере.");
                await DataSource.Save();
                await dialog.ShowAsync();
            }
            catch(Exception ex)
            {
                var dialog = new MessageDialog("Не удалось сохранить данные на сервере.");
                dialog.ShowAsync();
            }
            finally
            {
                synchProgressRing.IsActive = false;
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }
       

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await DataSource.Load();
                Frame.Navigate(typeof(ItemPage), activId);
            }
            catch (Exception ex)
            {
                Frame.Navigate(typeof(AuthorizationPage));
            }
            finally
            {

            }
        }

        private void Goals1ComboBoxEd_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");

            Goals1ComboBoxEd.Text = Regex.Replace(Goals1ComboBoxEd.Text, @"[^0-9]", "");
        }

        private void penalty1ComboBoxEd_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");

            penalty1ComboBoxEd.Text = Regex.Replace(penalty1ComboBoxEd.Text, @"[^0-9]", "");
        }

        private void Goals2ComboBoxEd_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");

            Goals2ComboBoxEd.Text = Regex.Replace(Goals2ComboBoxEd.Text, @"[^0-9]", "");
        }

        private void penalty2ComboBoxEd_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");

            penalty2ComboBoxEd.Text = Regex.Replace(penalty2ComboBoxEd.Text, @"[^0-9]", "");
        }

    }
    public class DateConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType,
            object parameter, string language)
        {

            string s = (string)value;
            DateTimeOffset dto = DateTimeOffset.Parse(s);
            return dto;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            DateTimeOffset dt = (DateTimeOffset)value;
            string s = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return s;
        }

        #endregion
    }
    public class TimeConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType,
            object parameter, string language)
        {

            string s = (string)value;
            TimeSpan tsp = TimeSpan.Parse(s);
            return tsp;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            TimeSpan tsp = (TimeSpan)value;
            string s = tsp.ToString(@"hh\:mm\:ss");
            return s;
        }

        #endregion
    }

}
