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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace UniversalApp_sportsandme
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InjuresAndIncidents : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private int ItemId;

        public InjuresAndIncidents()
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
            FootballMatch DataItem = await DataSource.GetItemAsync((int)e.NavigationParameter);
            ItemId = (int)e.NavigationParameter;
            if (DataItem.incident == null || DataItem.incident.deleted == true) 
            {
                IncidentPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                AddIncidentButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                
            }
            else
            {
                IncidentPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                AddIncidentButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                AcceptButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
             if (DataItem.injury == null || DataItem.injury.deleted == true) 
            {
                InjuryPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                AddInjuryButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                AddInjuryButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                InjuryPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                AcceptButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
             this.DefaultViewModel["Incidents"] = DataItem;

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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ItemPage), ItemId);
        }

        private async void AddInjuryButton_Click(object sender, RoutedEventArgs e)
        {
            var ds = await DataSource.GetItemAsync(ItemId);
            if (ds.injury == null)
            {
                ds.injury = new Injury(0);
            }
            else
            {
                ds.injury.deleted = false;
            }
            AddInjuryButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            InjuryPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            AcceptButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private async void AddIncidentButton_Click(object sender, RoutedEventArgs e)
        {
            var ds = await DataSource.GetItemAsync(ItemId);
            if (ds.incident == null)
            {
                ds.incident = new Incident(0);
            }
            else
            {
                ds.incident.deleted = false;
            }
            IncidentPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            AddIncidentButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AcceptButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private async void DeleteIncidentButton_Click(object sender, RoutedEventArgs e)
        {
            var ds = await DataSource.GetItemAsync(ItemId);
            ds.incident.deleted = true;
            IncidentPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AddIncidentButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private async void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            var ds = await DataSource.GetItemAsync(ItemId);
            if (ds.incident != null && ds.incident.deleted == false)
            {
                ds.incident.note = IncidentTextBox.Text;
            }
            if (ds.injury != null && ds.injury.deleted == false)
            {
                ds.injury.note = InjuryTextBox.Text;
            }
        }

        private async void DeleteInjuryButton_Click(object sender, RoutedEventArgs e)
        {
            var ds = await DataSource.GetItemAsync(ItemId);
            ds.injury.deleted = true;
            InjuryPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AddInjuryButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
    }
}
