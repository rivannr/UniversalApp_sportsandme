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
    public class Goal : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _objectStatus { get; set; }

        private int id { get; set; }
        private int match_id { get; set; }
        private int team_id { get; set; }
        private int player_id { get; set; }
        private int assistant_id { get; set; }
        private bool is_penalty { get; set; }
        private bool is_autogoal { get; set; }
        private int minute { get; set; }
        private int addition_minute { get; set; }

        public int objectStatus
        {
            get { return this._objectStatus; }
            set
            {
                if (value != this._objectStatus)
                {
                    this._objectStatus = value;
                    NotifyPropertyChanged();
                }
            }
        }
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
        public int Assistant_idValue
        {
            get { return this.assistant_id; }
            set
            {
                if (value != this.assistant_id)
                {
                    this.assistant_id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool Is_penaltyValue
        {
            get { return this.is_penalty; }
            set
            {
                if (value != this.is_penalty)
                {
                    this.is_penalty = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool Is_autogoalValue
        {
            get { return this.is_autogoal; }
            set
            {
                if (value != this.is_autogoal)
                {
                    this.is_autogoal = value;
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


        public Goal(int id, int match_id, int team_id, int player_id, int assistant_id, bool is_autogoal, bool is_penalty,  int minute, int addition_minute)
        {
            this.id = id;
            this.match_id = match_id;
            this.team_id = team_id;
            this.player_id = player_id;
            this.minute = minute;
            this.addition_minute = addition_minute;
            this.assistant_id = assistant_id;
            this.is_autogoal = is_autogoal;
            this.is_penalty = is_penalty;
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

        private async Task<bool> Get()
        {
            throw new Exception("NotImplemented");
            return true;

        }
        public async Task<HttpStatusCode> Post()
        {
            bool success = true;
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(DataSource.host + "goal" + "?access_token=" + DataSource.accessToken);
            List<KeyValuePair<string, string>> pairsToSend = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("match_id", this.match_id.ToString()),
                new KeyValuePair<string, string>("team_id", this.team_id.ToString()),
                new KeyValuePair<string, string>("player_id", this.player_id.ToString()),
                new KeyValuePair<string, string>("assistant_id", this.assistant_id.ToString()),
                new KeyValuePair<string, string>("is_penalty", this.is_penalty.ToString().ToLower()),
                new KeyValuePair<string, string>("is_autogoal", this.is_autogoal.ToString().ToLower()),
                new KeyValuePair<string, string>("minute", this.minute.ToString()),
                new KeyValuePair<string, string>("addition_minute", this.addition_minute.ToString()),
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
                success = false;
            }
            if (!success)
            {
                FootballMatch fm = await DataSource.GetItemAsync(this.match_id);
                objectStatus = (int)DataSource.status.needCreate;
                fm.goals.Add(this);
                throw new Exception("EnsureSuccessStatusCodeFailed");
            }
            string result = await response.Content.ReadAsStringAsync();
            JsonObject jsonObject = JsonObject.Parse(result);
            if (jsonObject.Count == 0)
            {
                
            }
            else
            {
                await DataSource.RefreshGoalsCollectionAsync(Match_idValue);  
            }
            objectStatus = (int)DataSource.status.ok;
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
            Uri uri = new Uri(DataSource.host + "goal/" + this.id.ToString() + "?access_token=" + DataSource.accessToken);
            List<KeyValuePair<string, string>> pairsToSend = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("match_id", this.match_id.ToString()),
                new KeyValuePair<string, string>("team_id", this.team_id.ToString()),
                new KeyValuePair<string, string>("player_id", this.player_id.ToString()),
                new KeyValuePair<string, string>("assistant_id", this.assistant_id.ToString()),
                new KeyValuePair<string, string>("is_penalty", this.is_penalty.ToString().ToLower()),
                new KeyValuePair<string, string>("is_autogoal", this.is_autogoal.ToString().ToLower()),
                new KeyValuePair<string, string>("minute", this.minute.ToString()),
                new KeyValuePair<string, string>("addition_minute", this.addition_minute.ToString()),
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
            }
            string result = await response.Content.ReadAsStringAsync();
            JsonObject jsonObject = JsonObject.Parse(result);

            objectStatus = (int)DataSource.status.ok;
            return response.StatusCode;
        }
        public async Task<HttpStatusCode> Delete()
        {
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(DataSource.host + "goal/" + this.id.ToString() + "?access_token=" + DataSource.accessToken);
            //var content = new FormUrlEncodedContent(pairsToSend);
            var response = await httpClient.DeleteAsync(uri);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                objectStatus = (int)DataSource.status.needDelete;
                throw new Exception(ex.Message);
            }
            objectStatus = (int)DataSource.status.Deleted;
            await DataSource.RefreshGoalsCollectionAsync(Match_idValue);
            return response.StatusCode;
        }
    }





}
