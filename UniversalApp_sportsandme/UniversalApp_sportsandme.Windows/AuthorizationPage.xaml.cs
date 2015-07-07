using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using System.Net.Http;
using UniversalApp_sportsandme.Common;
using UniversalApp_sportsandme.DataModel;
using System.Net;


// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace UniversalApp_sportsandme
{
    public sealed partial class AuthorizationPage : Page
    {

        public AuthorizationPage()
        {
            this.InitializeComponent();
            password.IsPasswordRevealButtonEnabled = true;

        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            loginButton.IsEnabled = false;
            try
            {
                HttpStatusCode responseCode = await DataSource.GetAccessKey(email.Text, password.Password);
                //if(responseCode == HttpStatusCode.OK){
                //}
                Frame.Navigate(typeof(TournamentsPage));
            }
            catch(Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                dialog.ShowAsync();
            }
            finally
            {
                loginButton.IsEnabled = true;
            }



        }



    }
}
