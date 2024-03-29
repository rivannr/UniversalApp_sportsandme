﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Text;
using UniversalApp_sportsandme.DataModel;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace UniversalApp_sportsandme.Data
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    /// 
    public static class GeneralApplicationData
    {

           public static string clientId = "6";
           public static string clientSecret = "4d3bc048b6f847ght9d5vf9fbc8q0m8x";
           public static string redirect_uri = "samapi://success";
           public static string host = "http://api.sportand.me:8000/";

           public static string accessToken { get; set; }
           public static string login { get; set; }

           public static HttpStatusCode lastResponseHttpStatusCode { get; set; }
    }
    public class SampleDataItem
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.Content = content;
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string Content { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.Items = new ObservableCollection<SampleDataItem>();
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public ObservableCollection<SampleDataItem> Items { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        //private static string accessToken = "";

        private ObservableCollection<SampleDataGroup> _groups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> Groups
        {
            get { return this._groups; }
        }

        public static async Task<IEnumerable<SampleDataGroup>> GetGroupsAsync()
        {
            await _sampleDataSource.GetSampleDataAsync();

            return _sampleDataSource.Groups;
        }

        public static async Task<SampleDataGroup> GetGroupAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<SampleDataItem> GetItemAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }
        public static async Task GetAccessKey(String login, String password)
        {
            HttpClient httpClient = new HttpClient();
            //string host = "http://api.sportand.me:8000/oauth/direct";
            //Uri uri = new Uri(host);
            //string postData = "client_id=6&client_secret=4d3bc048b6f847ght9d5vf9fbc8q0m8x&redirect_uri=samapi://success&login=rivannr@outlook.com&pass=31ker12";
            //StringContent sc = new StringContent(postData);
            Uri uri = new Uri(GeneralApplicationData.host + "oauth/direct");
            GeneralApplicationData.login = login;
            List<KeyValuePair<string, string>> pairsToSend = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", GeneralApplicationData.clientId),
                new KeyValuePair<string, string>("client_secret", GeneralApplicationData.clientSecret),
                new KeyValuePair<string, string>("redirect_uri", GeneralApplicationData.redirect_uri),
                new KeyValuePair<string, string>("login", GeneralApplicationData.login),
                new KeyValuePair<string, string>("pass", password)
            };

            var content = new FormUrlEncodedContent(pairsToSend);

            var response = await httpClient.PostAsync(uri, content);
            GeneralApplicationData.lastResponseHttpStatusCode = response.StatusCode;
            response.EnsureSuccessStatusCode();
            
 
            string result = await response.Content.ReadAsStringAsync();
            JsonObject jsonObject = JsonObject.Parse(result);
            GeneralApplicationData.accessToken = jsonObject["access_token"].GetString();
           // return null;
        }
        private async Task GetGeneralApplicationData(String login, String password)
        {
            
            //await GetAccessKey(pairsToSend, uri);
        }
        private async Task GetSampleDataAsync()
        {
            if (this._groups.Count != 0)
                return;
            ////
            ////
            //await GetAccessKey();
            ////
            ////
            Uri dataUri = new Uri("ms-appx:///DataModel/SampleData/SampleData.json");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Groups"].GetArray();

            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();
                SampleDataGroup group = new SampleDataGroup(groupObject["UniqueId"].GetString(),
                                                            groupObject["Title"].GetString(),
                                                            groupObject["Subtitle"].GetString(),
                                                            groupObject["ImagePath"].GetString(),
                                                            groupObject["Description"].GetString());

                foreach (JsonValue itemValue in groupObject["Items"].GetArray())
                {
                    JsonObject itemObject = itemValue.GetObject();
                    group.Items.Add(new SampleDataItem(itemObject["UniqueId"].GetString(),
                                                       itemObject["Title"].GetString(),
                                                       itemObject["Subtitle"].GetString(),
                                                       itemObject["ImagePath"].GetString(),
                                                       itemObject["Description"].GetString(),
                                                       itemObject["Content"].GetString()));
                }
                this.Groups.Add(group);
            }
        }
    }
}