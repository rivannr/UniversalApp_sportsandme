using System;
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
using Newtonsoft.Json;
using Windows.Storage.Streams;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UniversalApp_sportsandme.DataModel
{
    public class RedCard : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int objectStatus { get; set; }

        private int id { get; set; }
        private int match_id { get; set; }
        private int team_id { get; set; }
        private int player_id { get; set; }
        private bool is_two_yellow { get; set; }
        private int minute { get; set; }
        private int addition_minute { get; set; }
        private string note { get; set; }

        public int IdValue
        {
            get { return this.id; }
            set
            {
                if (value != this.id)
                {
                    this.id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool Is_two_yellowValue
        {
            get { return this.is_two_yellow; }
            set
            {
                if (value != this.is_two_yellow)
                {
                    this.is_two_yellow = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Match_idValue
        {
            get { return this.match_id; }
            set
            {
                if (value != this.match_id)
                {
                    this.match_id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Team_idValue
        {
            get { return this.team_id; }
            set
            {
                if (value != this.team_id)
                {
                    this.team_id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Player_idValue
        {
            get { return this.player_id; }
            set
            {
                if (value != this.player_id)
                {
                    this.player_id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int MinuteValue
        {
            get { return this.minute; }
            set
            {
                if (value != this.minute)
                {
                    this.minute = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Addition_minuteValue
        {
            get { return this.addition_minute; }
            set
            {
                if (value != this.addition_minute)
                {
                    this.addition_minute = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string NoteValue
        {
            get { return this.note; }
            set
            {
                if (value != this.note)
                {
                    this.note = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public RedCard(int id, int match_id, int team_id, int player_id, bool is_two_yellow, int minute, int addition_minute, string note)
        {
            this.id = id;
            this.match_id = match_id;
            this.team_id = team_id;
            this.player_id = player_id;
            this.minute = minute;
            this.addition_minute = addition_minute;
            this.note = note;
            this.is_two_yellow = is_two_yellow;
        }
        public async Task<HttpStatusCode> Update()
        {
            HttpStatusCode b;
            switch (objectStatus)
            {
                case (int)DataSource.status.needCreate:
                    b = await Post();
                    break;
                case (int)DataSource.status.needDelete:
                    b = await Delete();
                    break;
                case (int)DataSource.status.needUpdate:
                    b = await Patch();
                    break;
                default:
                    b = HttpStatusCode.BadRequest;
                    break;
            }
            return b;
        }
        /// <summary>
        /// Uses a first element that not part of local collection to identify this particularly object 
        /// You should replace it with a method that would be reload whole collection for match if it possible to create multiple object in same time
        /// </summary>
        /// <returns>Returns True if Success.</returns>
        private async Task<HttpStatusCode> Get()
        {
            throw new Exception("NotImplemented");
            return HttpStatusCode.NotImplemented;
        }
        public async Task<HttpStatusCode> Post()
        {
            bool success = true;
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(DataSource.host + "red_card" + "?access_token=" + DataSource.accessToken);
            List<KeyValuePair<string, string>> pairsToSend = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("match_id", this.match_id.ToString()),
                new KeyValuePair<string, string>("team_id", this.team_id.ToString()),
                new KeyValuePair<string, string>("player_id", this.player_id.ToString()),
                new KeyValuePair<string, string>("is_two_yellow", this.is_two_yellow.ToString().ToLower()),
                new KeyValuePair<string, string>("minute", this.minute.ToString()),
                new KeyValuePair<string, string>("addition_minute", this.addition_minute.ToString()),
                new KeyValuePair<string, string>("note", this.note),
            };
            var content = new FormUrlEncodedContent(pairsToSend);
            var response = await httpClient.PostAsync(uri, content);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                success = false;
                
            }
            if (!success)
            {
                FootballMatch fm = await DataSource.GetItemAsync(this.match_id);

                objectStatus = (int)DataSource.status.needCreate;
                fm.redcards.Add(this);
                
            }
            string result = await response.Content.ReadAsStringAsync();
            JsonObject jsonObject = JsonObject.Parse(result);
            objectStatus = (int)DataSource.status.ok;
            await DataSource.RefreshRedCardsCollectionAsync(Match_idValue);  
            return response.StatusCode;
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public async Task<HttpStatusCode> Patch()
        {
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(DataSource.host + "red_card/" + this.id.ToString() + "?access_token=" + DataSource.accessToken);
            List<KeyValuePair<string, string>> pairsToSend = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("match_id", this.match_id.ToString()),
                new KeyValuePair<string, string>("team_id", this.team_id.ToString()),
                new KeyValuePair<string, string>("player_id", this.player_id.ToString()),
                new KeyValuePair<string, string>("is_two_yellow", this.is_two_yellow.ToString().ToLower()),
                new KeyValuePair<string, string>("minute", this.minute.ToString()),
                new KeyValuePair<string, string>("addition_minute", this.addition_minute.ToString()),
                new KeyValuePair<string, string>("note", this.note),
            };
            var content = new FormUrlEncodedContent(pairsToSend);
            var response = await httpClient.PatchAsync(uri, content);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                objectStatus = (int)DataSource.status.needUpdate;
            }
            string result = await response.Content.ReadAsStringAsync();
            JsonObject jsonObject = JsonObject.Parse(result);
            if (jsonObject.Count == 0)
            {
                //
            }
            else
            {
                // this.id = (int)jsonObject["id"].GetNumber();
            }
            objectStatus = (int)DataSource.status.ok;
            await DataSource.RefreshRedCardsCollectionAsync(Match_idValue);  
            return response.StatusCode;
        }
        public async Task<HttpStatusCode> Delete()
        {
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(DataSource.host + "red_card/" + this.id.ToString() + "?access_token=" + DataSource.accessToken);
            var response = await httpClient.DeleteAsync(uri);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                objectStatus = (int)DataSource.status.needDelete;
                
            }
            objectStatus = (int)DataSource.status.Deleted;
            await DataSource.RefreshRedCardsCollectionAsync(Match_idValue);
            return response.StatusCode;
        }
    }





}
