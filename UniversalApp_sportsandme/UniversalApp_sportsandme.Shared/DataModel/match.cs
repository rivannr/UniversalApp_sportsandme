using System;
using System.Collections.Generic;
using System.Text;
using UniversalApp_sportsandme.Common;
using System.Collections.ObjectModel;
using Windows.Storage.Streams;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace UniversalApp_sportsandme.DataModel
{


    public class FootballMatch : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool actual { get; set; }

        public int id { get; set; }
        public int round_id { get; set; }
        public int team1_id { get; set; }
        public int team2_id { get; set; }
        private int _goals1 { get; set; }
        public int goals1
        {
            get { return this._goals1; }
            set
            {
                if (value != this._goals1)
                {
                    this._goals1 = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _goals2 { get; set; }
        public int goals2
        {
            get { return this._goals2; }
            set
            {
                if (value != this._goals2)
                {
                    this._goals2 = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _penalty1 { get; set; }
        public int penalty1
        {
            get { return this._penalty1; }
            set
            {
                if (value != this._penalty1)
                {
                    this._penalty1 = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _penalty2 { get; set; }
        public int penalty2
        {
            get { return this._penalty2; }
            set
            {
                if (value != this._penalty2)
                {
                    this._penalty2 = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _is_technical { get; set; }
        public bool is_technical
        {
            get { return this._is_technical; }
            set
            {
                if (value != this._is_technical)
                {
                    this._is_technical = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _is_overtime { get; set; }
        public bool is_overtime
        {
            get { return this._is_overtime; }
            set
            {
                if (value != this._is_overtime)
                {
                    this._is_overtime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _is_shootout { get; set; }
        public bool is_shootout
        {
            get { return this._is_shootout; }
            set
            {
                if (value != this._is_shootout)
                {
                    this._is_shootout = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string next_id { get; set; }
        public string tour_id { get; set; }
        public string tree_id { get; set; }
        public string referee { get; set; }
        private string _start_at { get; set; }
        public string start_at
        {
            get { return this._start_at; }
            set
            {
                if (value != this._start_at)
                {
                    this._start_at = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string time_zone { get; set; }
        private string _place { get; set; }
        public string place
        {
            get { return this._place; }
            set
            {
                if (value != this._place)
                {
                    this._place = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Addition addition { get; set; }
        public Injury injury { get; set; }
        public Incident incident { get; set; }

        public FootballTeam team1 { get; set; }
        public FootballTeam team2 { get; set; }

        public ObservableCollection<MatchPlayer> matchPlayers { get; set; }
        public ObservableCollection<Player> team1_players { get; set; }
        public ObservableCollection<Player> team2_players { get; set; }
        public ObservableCollection<YellowCard> yellowcards { get; set; }
        public ObservableCollection<RedCard> redcards { get; set; }
        public ObservableCollection<Goal> goals { get; set; }

        public FootballMatch(int id, bool actual)
        {
            this.id = id;
            this.actual = actual;
            matchPlayers = new ObservableCollection<MatchPlayer>();
            team1_players = new ObservableCollection<Player>();
            team2_players = new ObservableCollection<Player>();
            yellowcards = new ObservableCollection<YellowCard>();
            redcards = new ObservableCollection<RedCard>();
            goals = new ObservableCollection<Goal>();
        }
        public async Task UpdateMatchInfo()
        {
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(DataSource.host + "match/" + this.id.ToString() + "?access_token=" + DataSource.accessToken);
            List<KeyValuePair<string, string>> pairsToSend = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("goals1", this.goals1.ToString()),
                new KeyValuePair<string, string>("goals2", this.goals2.ToString()),
                new KeyValuePair<string, string>("penalty1", this.penalty1.ToString()),
                new KeyValuePair<string, string>("penalty2", this.penalty2.ToString()),
                new KeyValuePair<string, string>("start_at", this.start_at),
                new KeyValuePair<string, string>("place", this.place),
            };
            var content = new FormUrlEncodedContent(pairsToSend);
            var response = await httpClient.PatchAsync(uri, content);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                //objectStatus = (int)DataSource.status.needUpdate;
                throw new Exception(ex.Message);
                //return false;
            }
            string result = await response.Content.ReadAsStringAsync();
            //JsonObject jsonObject = JsonObject.Parse(result);
            //if (jsonObject.Count == 0)
            //{
               // return false;
            //}
            //else
            //{
                // this.id = (int)jsonObject["id"].GetNumber();
            //}
           // objectStatus = (int)DataSource.status.ok;
           // await DataSource.RefreshYellowCardsCollectionAsync(Match_idValue);
            //return true;
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class FootballTeam
    {
        public int id { get; set; }
        public string type_id { get; set; }
        public string category_id { get; set; }
        public string user_id { get; set; }
        public string title { get; set; }
        public string image_path { get; set; }
        public string cover_photo_id { get; set; }
        public string cover_photo_position { get; set; }
        public string country_id { get; set; }
        public string region_id { get; set; }
        public string city_id { get; set; }
        public string page_vk { get; set; }
        public string page_fb { get; set; }
        public string page_tw { get; set; }
        public string page_in { get; set; }

        public FootballTeam(int id)
        {
            this.id = id;
        }
    }


    public class ApplicantsList
    {
        public int id { get; set; }
        public int team_id { get; set; }
        public int member_id { get; set; }
        public bool approved { get; set; }
        public bool active { get; set; }
        public Player player { get; set; }
        public object coach { get; set; }
        public bool returned { get; set; }
        public string _in { get; set; }
        public string _out { get; set; }
    }

    
    public class Player
    {
        public int id { get; set; }
        public string full_name { get; set; }
        public string image_path { get; set; }
        public string position { get; set; }
        public int gender { get; set; }
        public int birthday { get; set; }
        public int number { get; set; }
        public int user_id { get; set; }

        public Player(int id, string full_name, string image_path, string position, int gender, int number)
        {
            this.id = id;
            this.full_name = full_name;
            this.image_path = image_path;
            this.position = position;
            this.gender = gender;
            this.number = number;
        }
    }

}
