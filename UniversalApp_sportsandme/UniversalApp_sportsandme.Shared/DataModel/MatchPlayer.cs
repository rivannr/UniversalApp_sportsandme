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
    public class MatchPlayer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int objectStatus { get; set; }
        private int id;
        private int match_id;
        private int team_id;
        private int player_id;
        private int teamsheet;
        private bool is_capitan;
        private bool is_goalkeeper;

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
        public int TeamsheetValue
        {
            get { return teamsheet; }
            set
            {
                if (value != this.teamsheet && value != -1)
                {
                    this.teamsheet = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool Is_capitanValue
        {
            get { return this.is_capitan; }
            set
            {
                if (value != this.is_capitan)
                {
                    this.is_capitan = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool Is_goalkeeperValue
        {
            get { return this.is_goalkeeper; }
            set
            {
                if (value != this.is_goalkeeper)
                {
                    this.is_goalkeeper = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public MatchPlayer(int id, int match_id, int team_id, int player_id)
        {
            teamsheet = 0;
            is_capitan = false;
            is_goalkeeper = false;
            this.id = id;
            this.match_id = match_id;
            this.team_id = team_id;
            this.player_id = player_id;
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
            FootballMatch fm = await DataSource.GetItemAsync(this.match_id);
            HttpClient httpClient = new HttpClient();
            string urlMatchPlayers = DataSource.host + "match/";
            Uri uriMatchPlayers = new Uri(urlMatchPlayers + this.match_id.ToString() + "/match_player?access_token=" + DataSource.accessToken + "&"+ Guid.NewGuid().ToString());

            var responseMatchPlayers = await httpClient.GetAsync(uriMatchPlayers);

            try
            {
                responseMatchPlayers.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return false;
            }
            string resultMatchPlayers = await responseMatchPlayers.Content.ReadAsStringAsync();
            JsonObject jsonObjectMatchPlayers = JsonObject.Parse(resultMatchPlayers);

            try
            {
                JsonArray jap = jsonObjectMatchPlayers["rows"].GetArray();
                if (jap.Count > 0)
                {
                    foreach (JsonValue jsonValueApplicant in jap)
                    {
                        bool b = true;
                        JsonObject japMatchPlayer = jsonValueApplicant.GetObject();
                        int _id = 0;
                        int _match_id = 0;
                        int _team_id = 0;
                        if (japMatchPlayer["id"].ValueType.Equals(JsonValueType.Number))
                        {
                            _id = (int)japMatchPlayer["id"].GetNumber();
                        }
                        if (japMatchPlayer["match_id"].ValueType.Equals(JsonValueType.Number))
                        {
                            _match_id = (int)japMatchPlayer["match_id"].GetNumber();
                        }
                        if (japMatchPlayer["team_id"].ValueType.Equals(JsonValueType.Number))
                        {
                            _team_id = (int)japMatchPlayer["team_id"].GetNumber();
                        }
                        foreach( MatchPlayer mpl in fm.matchPlayers){
                            if (mpl.id == _id)
                            {
                                b = false;
                            }
                        }
                        if (b && _match_id == this.match_id)
                        {
                            if (_team_id == this.team_id)
                            {
                                this.id = _id;
                                fm.matchPlayers.Add(this);
                            }
                        }
                        
                    }
                }
            }


            catch (Exception ex)
            {

            }
            return true;

        }
        public async Task<bool> Post()
        {
            bool success = true;
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(DataSource.host + "match_player" + "?access_token=" + DataSource.accessToken);
            List<KeyValuePair<string, string>> pairsToSend = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("match_id", this.match_id.ToString()),
                new KeyValuePair<string, string>("team_id", this.team_id.ToString()),
                new KeyValuePair<string, string>("player_id", this.player_id.ToString()),
                new KeyValuePair<string, string>("teamsheet", this.teamsheet.ToString()),
                new KeyValuePair<string, string>("is_capitan", this.is_capitan.ToString().ToLower()),
                new KeyValuePair<string, string>("is_goalkeeper", this.is_goalkeeper.ToString().ToLower()),
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
                fm.matchPlayers.Add(this);
                throw new Exception(response.StatusCode.ToString());
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
                await Get();
            }
            objectStatus = (int)DataSource.status.ok;
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
            Uri uri = new Uri(DataSource.host + "match_player/" + this.id.ToString() + "?access_token=" + DataSource.accessToken);
            List<KeyValuePair<string, string>> pairsToSend = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("match_id", this.match_id.ToString()),
                new KeyValuePair<string, string>("team_id", this.team_id.ToString()),
                new KeyValuePair<string, string>("player_id", this.player_id.ToString()),
                new KeyValuePair<string, string>("teamsheet", this.teamsheet.ToString()),
                new KeyValuePair<string, string>("is_capitan", this.is_capitan.ToString().ToLower()),
                new KeyValuePair<string, string>("is_goalkeeper", this.is_goalkeeper.ToString().ToLower()),
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
            return true;
        }
        public async Task<bool> Delete()
        {
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(DataSource.host + "match_player/" + this.id.ToString() + "?access_token=" + DataSource.accessToken);
            var response = await httpClient.DeleteAsync(uri);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                objectStatus = (int)DataSource.status.needDelete;
                throw new Exception(ex.Message);
                return false;
            }
            objectStatus = (int)DataSource.status.Deleted;
            return true;
        }
    }





}
