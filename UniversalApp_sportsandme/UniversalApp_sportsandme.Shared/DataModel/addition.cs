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

namespace UniversalApp_sportsandme.DataModel
{
    public class Addition
    {
        public Addition(int id)
        {
            this.id = id;
            deleted = false;
            objectStatus = 0;
            
        }
        public bool deleted { get; set; }
        public int objectStatus { get; set; }
        
        public int id { get; set; }
        //public int match_id { get; set; }
        public int starthour { get; set; }
        public int startminute { get; set; }
        public int endhour { get; set; }
        public int endminute { get; set; }
        public int half_time_minutes { get; set; }
        public int attendance { get; set; }
        public int match_number { get; set; }

        public async Task<bool> Update(int matchId)
        {
            bool b = false;
            switch (objectStatus)
            {
                case (int)DataSource.status.needCreate:
                    b =  await Post(matchId);
                    break;
                case (int)DataSource.status.needDelete:
                    b = await Delete(matchId);
                    break;
                case (int)DataSource.status.needUpdate:
                    b = await Patch(matchId);
                    break;
                default:
                    b = true;
                    break;
            }
            return b;
        }
        public async Task<bool> Get(int MatchId)
        {
            //•GET /match/39/addition - выборка доп. информации, по id матча
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(DataSource.host + "match/" + MatchId.ToString() + "/addition?access_token=" + DataSource.accessToken);
            var response = await httpClient.GetAsync(uri);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return false;
            }
            string result = await response.Content.ReadAsStringAsync();
            JsonObject jsonObject = JsonObject.Parse(result);
            if (jsonObject.Count == 0)
            {
                return false;
            }
            this.id = (int)jsonObject["id"].GetNumber();
            this.starthour = (int)jsonObject["starthour"].GetNumber();
            this.startminute = (int)jsonObject["startminute"].GetNumber();
            this.endhour = (int)jsonObject["endhour"].GetNumber();
            this.endminute = (int)jsonObject["endminute"].GetNumber();
            this.half_time_minutes = (int)jsonObject["half_time_minutes"].GetNumber();
            this.attendance = (int)jsonObject["attendance"].GetNumber();
            this.match_number = (int)(jsonObject["match_number"].GetNumber());
            return true;
        }
        public async Task<bool> Post(int MatchId)
        {
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(DataSource.host + "match/" + MatchId.ToString() + "/addition?access_token=" + DataSource.accessToken);
            List<KeyValuePair<string, string>> pairsToSend = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("starthour", this.starthour.ToString()),
                new KeyValuePair<string, string>("startminute", this.startminute.ToString()),
                new KeyValuePair<string, string>("endhour", this.endhour.ToString()),
                new KeyValuePair<string, string>("endminute", this.endminute.ToString()),
                new KeyValuePair<string, string>("half_time_minutes", this.half_time_minutes.ToString()),
                new KeyValuePair<string, string>("attendance", this.attendance.ToString()),
                new KeyValuePair<string, string>("match_number", this.match_number.ToString()),
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
                return false;
            }
            string result = await response.Content.ReadAsStringAsync();
            JsonObject jsonObject = JsonObject.Parse(result);

            objectStatus = (int)DataSource.status.ok;
            return true;
        }
        public async Task<bool> Patch(int MatchId)
        {
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(DataSource.host + "match/" + MatchId.ToString() + "/addition?access_token=" + DataSource.accessToken);
            List<KeyValuePair<string, string>> pairsToSend = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("starthour", this.starthour.ToString()),
                new KeyValuePair<string, string>("startminute", this.startminute.ToString()),
                new KeyValuePair<string, string>("endhour", this.endhour.ToString()),
                new KeyValuePair<string, string>("endminute", this.endminute.ToString()),
                new KeyValuePair<string, string>("half_time_minutes", this.half_time_minutes.ToString()),
                new KeyValuePair<string, string>("attendance", this.attendance.ToString()),
                new KeyValuePair<string, string>("match_number", this.match_number.ToString()),
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
        public async Task<bool> Delete(int MatchId)
        {
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(DataSource.host + "match/" + MatchId.ToString() + "/addition?access_token=" + DataSource.accessToken);
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
            return true;
        }
    }





}
