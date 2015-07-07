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
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace UniversalApp_sportsandme
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdditionPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private int activId;

        public AdditionPage()
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
                var DataGroups = await DataSource.GetAdditionAsync((int)e.NavigationParameter);
                if (DataGroups == null || DataGroups.objectStatus == (int)DataSource.status.Deleted)
                {
                    CreateNew.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    dataPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    DeleteButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    BackButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    CreateNew.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    dataPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    DeleteButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    BackButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                activId = (int)e.NavigationParameter;
                if (DataGroups != null)
                {
                    StartClock.Time = new TimeSpan(DataGroups.starthour, DataGroups.startminute, 0);
                    EndClock.Time = new TimeSpan(DataGroups.endhour, DataGroups.endminute, 0);
                    this.DefaultViewModel["Addition"] = DataGroups;
                }
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

        private void NumberTextBoxBreaktime_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");

            NumberTextBoxBreaktime.Text = Regex.Replace(NumberTextBoxBreaktime.Text, @"[^0-9]", "");

        }

        private void NumberTextBoxAttendance_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");

            NumberTextBoxAttendance.Text = Regex.Replace(NumberTextBoxAttendance.Text, @"[^0-9]", "");
        }

        private void NumberTextBoxMatchNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");

            NumberTextBoxMatchNumber.Text = Regex.Replace(NumberTextBoxMatchNumber.Text, @"[^0-9]", "");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ItemPage), activId);
        }

        private async void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Visible;
                synchProgressRing.IsActive = true;
                    ((Addition)(this.DefaultViewModel["Addition"])).match_number = System.Int32.Parse(NumberTextBoxMatchNumber.Text);
                    ((Addition)(this.DefaultViewModel["Addition"])).attendance = System.Int32.Parse(NumberTextBoxAttendance.Text);
                    ((Addition)(this.DefaultViewModel["Addition"])).half_time_minutes = System.Int32.Parse(NumberTextBoxBreaktime.Text);
                    ((Addition)(this.DefaultViewModel["Addition"])).endhour = EndClock.Time.Hours;
                    ((Addition)(this.DefaultViewModel["Addition"])).endminute = EndClock.Time.Minutes;
                    ((Addition)(this.DefaultViewModel["Addition"])).starthour = StartClock.Time.Hours;
                    ((Addition)(this.DefaultViewModel["Addition"])).startminute = StartClock.Time.Minutes;
  
                bool b = false;
                if (((Addition)(this.DefaultViewModel["Addition"])).objectStatus == (int)DataSource.status.needCreate)
                {
                    await ((Addition)this.DefaultViewModel["Addition"]).Post(activId);
                }
                else
                {
                    await ((Addition)(this.DefaultViewModel["Addition"])).Patch(activId);

                }
                
                    var dialog = new MessageDialog("Изменения успешно  сохранены на сервере");
                    await DataSource.Save();
                    await dialog.ShowAsync();

            }

            catch (Exception parseEx)
            {
                     var dialog = new MessageDialog("Данные не переданы на сервер и добавлены в очередь. Повторите позже или используйте общую синхронизацию");
                     dialog.ShowAsync();
            }
            finally
            {
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                synchProgressRing.IsActive = false;
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            bool b = false;
            try
            {
                loadingRectangle.Visibility = Windows.UI.Xaml.Visibility.Visible;
                synchProgressRing.IsActive = true;
                var dm = await DataSource.GetAdditionAsync(activId);
                dm.deleted = true;
                CreateNew.Visibility = Windows.UI.Xaml.Visibility.Visible;
                dataPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                DeleteButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                BackButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                
                if (((Addition)(this.DefaultViewModel["Addition"])) != null && (((Addition)(this.DefaultViewModel["Addition"])).objectStatus == (int)DataSource.status.ok))
                {
                    await ((Addition)(this.DefaultViewModel["Addition"])).Delete(activId);
                    //((Addition)(this.DefaultViewModel["Addition"])).deleted = true;
                }
                if (((Addition)(this.DefaultViewModel["Addition"])) != null && (((Addition)(this.DefaultViewModel["Addition"])).objectStatus == (int)DataSource.status.needCreate))
                {
                    ((Addition)(this.DefaultViewModel["Addition"])).objectStatus = (int)DataSource.status.Deleted;
                    
                }
                var dialog = new MessageDialog("Изменения успешно  сохранены на сервере.");
                await DataSource.Save();
                await dialog.ShowAsync();
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

        private async void CreateNew_Click(object sender, RoutedEventArgs e)
        {
                var dm = await DataSource.GetItemAsync(activId);
                if (dm.addition == null)
                {
                    dm.addition = new Addition(0);
                }
                else
                {
                    if(dm.addition.objectStatus == (int)DataSource.status.needDelete){
                        dm.addition.objectStatus = (int)DataSource.status.ok;
                    }
                    if (dm.addition.objectStatus == (int)DataSource.status.Deleted)
                    {
                        dm.addition.objectStatus = (int)DataSource.status.needCreate;
                    }
                }
            this.DefaultViewModel["Addition"] = dm.addition;
            CreateNew.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            dataPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            DeleteButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            BackButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }


    }
}
