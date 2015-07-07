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
    public class YellowCard : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int objectStatus { get; set; }

        private int id { get; set; }
        private int match_id { get; set; }
        private int team_id { get; set; }
        private int player_id { get; set; }
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

        public YellowCard(int id, int match_id, int team_id, int player_id, int minute, int addition_minute, string note)
        {
            this.id = id;
            this.match_id = match_id;
            this.team_id = team_id;
            this.player_id = player_id;
            this.minute = minute;
            this.addition_minute = addition_minute;
            this.note = note;
        }
        public async Task<bool> Update()
        {
            bool b = false;
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
                    b = true;
                    break;
            }
            return b;
        }
        private async Task<bool> Get()
        {
            throw new Exception("NotImplemented");
            
            return true;

        }
        public async Task<bool> Post()
        {
            bool success = true;
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(DataSource.host + "yellow_card" + "?access_token=" + DataSource.accessToken);
            List<KeyValuePair<string, string>> pairsToSend = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("match_id", this.match_id.ToString()),
                new KeyValuePair<string, string>("team_id", this.team_id.ToString()),
                new KeyValuePair<string, string>("player_id", this.player_id.ToString()),
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
                objectStatus = (int)DataSource.status.needCreate;
                throw new Exception(ex.Message);
                success = false;  
            }
           // if (!success)
           // {
           //     FootballMatch fm = await DataSource.GetItemAsync(this.match_id);
            //
            //    objectStatus = (int)DataSource.status.needCreate;
            //    fm.yellowcards.Add(this);
            //    return false;
            //}
            string result = await response.Content.ReadAsStringAsync();
            JsonObject jsonObject = JsonObject.Parse(result);
            if (jsonObject.Count == 0)
            {
                return false;
            }
            else
            {
                //await Get();

                //JsonArray jap = jsonObject["rows"].GetArray();
                // if (jap.Count > 0)
                // {
                //     foreach (JsonValue jsonValueApplicant in jap)
                //     {
                //         JsonObject japMatchPlayer = jsonValueApplicant.GetObject();
                //         if (japMatchPlayer["id"].ValueType.Equals(JsonValueType.Number)) this.id = (int)japMatchPlayer["id"].GetNumber();
                //     }
                // }
            }
            objectStatus = (int)DataSource.status.ok;
            await DataSource.RefreshYellowCardsCollectionAsync(Match_idValue); 
            return true;
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public async Task<bool> Patch()
        {
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(DataSource.host + "yellow_card/" + this.id.ToString() + "?access_token=" + DataSource.accessToken);
            List<KeyValuePair<string, string>> pairsToSend = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("match_id", this.match_id.ToString()),
                new KeyValuePair<string, string>("team_id", this.team_id.ToString()),
                new KeyValuePair<string, string>("player_id", this.player_id.ToString()),
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
                throw new Exception(ex.Message);
                return false;
            }
            string result = await response.Content.ReadAsStringAsync();
            JsonObject jsonObject = JsonObject.Parse(result);
            if (jsonObject.Count == 0)
            {
                return false;
            }
            else
            {
                // this.id = (int)jsonObject["id"].GetNumber();
            }
            objectStatus = (int)DataSource.status.ok;
            await DataSource.RefreshYellowCardsCollectionAsync(Match_idValue); 
            return true;
        }
        public async Task<bool> Delete()
        {
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(DataSource.host + "yellow_card/" + this.id.ToString() + "?access_token=" + DataSource.accessToken);
            var response = await httpClient.DeleteAsync(uri);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                objectStatus = (int)DataSource.status.needDelete;
                return false;
            }
            objectStatus = (int)DataSource.status.Deleted;
            await DataSource.RefreshYellowCardsCollectionAsync(Match_idValue);  
            return true;
        }
    }





}
