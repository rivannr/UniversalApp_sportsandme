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
    public sealed class DataSource
    {
        public static bool busy = false;
        public enum status { ok, needUpdate, needDelete, needCreate, Deleted };

        public static string clientId = "6";
        public static string clientSecret = "4d3bc048b6f847ght9d5vf9fbc8q0m8x";
        public static string redirect_uri = "samapi://success";
        public static string host = "http://api.sportand.me/";

        public static string accessToken { get; set; }
        public static string refreshToken { get; set; }
        public static string login { get; set; }
        public static string expire_at { get; set; }

        public String _accessToken { get; set; }
        public String _refreshToken { get; set; }
        public String _login { get; set; }
        public String _expire_at { get; set; }

        private static DataSource _DataSource = new DataSource();
        public static HttpClient httpClient = new HttpClient();

        private ObservableCollection<Tournament> _tournaments = new ObservableCollection<Tournament>();

        public ObservableCollection<Tournament> Tournaments
        {
            get { return this._tournaments; }
        }

        public static async Task<IEnumerable<FootballMatch>> GetItemsAsync(int uniqueId)
        {
            await _DataSource.GetRefereeMatchCollectionAsync();
            // Simple linear search is acceptable for small data sets
            var roundMatches = _DataSource.Tournaments.SelectMany(tournament => tournament.RoundsList).Where((round) => round.id.Equals(uniqueId));
            var matches = roundMatches.SelectMany(round => round.Items);//_DataSource.Tournaments.SelectMany(tournament => tournament.RoundsList).SelectMany(round => round.Items);
            if (matches.Count() > 0) return matches;
            return null;
        }

        public static async Task<Addition> GetAdditionAsync(int uniqueId)
        {
            await _DataSource.GetRefereeMatchCollectionAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _DataSource.Tournaments.SelectMany(tournament => tournament.RoundsList).SelectMany(round => round.Items).Where((item) => item.id.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First().addition;
            return null;
        }

        public static async Task<FootballMatch> GetItemAsync(int uniqueId)
        {
            await _DataSource.GetRefereeMatchCollectionAsync();
            // Simple linear search is acceptable for small data sets
            //var matches = _DataSource.Groups.SelectMany(group => group.Items).Where((item) => item.id.Equals(uniqueId));
            var matches = _DataSource.Tournaments.SelectMany(tournament => tournament.RoundsList).SelectMany(round => round.Items).Where((item) => item.id.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }


        public static async Task<Round> GetRoundAsync(int uniqueId)
        {
            await _DataSource.GetRefereeMatchCollectionAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _DataSource.Tournaments.SelectMany(tournament => tournament.RoundsList).Where((round) => round.id.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }
        public static async Task<IEnumerable<Tournament>> GetTournamentsAsync()
        {
            await _DataSource.GetRefereeMatchCollectionAsync();

            return _DataSource.Tournaments;
        }
        public static async Task RefreshGoalsCollectionAsync(int matchId)
        {
            FootballMatch fm = await DataSource.GetItemAsync(matchId);
            if (fm != null)
            {
                fm.goals.Clear();
                await _DataSource.GetGoals(matchId, fm);
            }
        }
        public static async Task RefreshRedCardsCollectionAsync(int matchId)
        {
            FootballMatch fm = await DataSource.GetItemAsync(matchId);
            if (fm != null)
            {
                fm.redcards.Clear();
                await _DataSource.GetRedCards(matchId, fm);
            }
        }
        public static async Task RefreshYellowCardsCollectionAsync(int matchId)
        {
            FootballMatch fm = await DataSource.GetItemAsync(matchId);
            if (fm != null)
            {
                fm.yellowcards.Clear();
                await _DataSource.GetYellowCards(matchId, fm);
            }
        }
        public static async Task<Tournament> GetTournamentAsync(int uniqueId)
        {
            await _DataSource.GetRefereeMatchCollectionAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _DataSource.Tournaments.Where((tournament) => tournament.id.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        private async Task<HttpStatusCode> GetMatchPlayers(int matchId, FootballMatch match)
        {
            //api.sportand.me:8000/team/16429/applicant?access_token=xxx&tournament_id=1728
            string urlMatchPlayers = DataSource.host + "match/";
            Uri uriMatchPlayers = new Uri(urlMatchPlayers + match.id.ToString() + "/match_player?access_token=" + DataSource.accessToken + "&" + Guid.NewGuid().ToString());

            var responseMatchPlayers = await httpClient.GetAsync(uriMatchPlayers);
            //try
            //{
            responseMatchPlayers.EnsureSuccessStatusCode();
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            string resultMatchPlayers = await responseMatchPlayers.Content.ReadAsStringAsync();
            JsonObject jsonObjectMatchPlayers = JsonObject.Parse(resultMatchPlayers);

            //try
            //{
            JsonArray jap = jsonObjectMatchPlayers["rows"].GetArray();
            if (jap.Count > 0)
            {
                foreach (JsonValue jsonValueApplicant in jap)
                {
                    JsonObject japMatchPlayer = jsonValueApplicant.GetObject();
                    int _match_id = 0;
                    int _team_id = 0;
                    int _player_id = 0;
                    int _id = 0;
                    int _teamsheet = 0;
                    bool _isCaptain = false;
                    bool _isGoalkeeper = false;
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
                    if (japMatchPlayer["player_id"].ValueType.Equals(JsonValueType.Number))
                    {
                        _player_id = (int)japMatchPlayer["player_id"].GetNumber();
                    }
                    if (japMatchPlayer["teamsheet"].ValueType.Equals(JsonValueType.Number))
                    {
                        _teamsheet = (int)japMatchPlayer["teamsheet"].GetNumber();
                    }
                    if (japMatchPlayer["is_capitan"].ValueType.Equals(JsonValueType.Boolean))
                    {
                        _isCaptain = japMatchPlayer["is_capitan"].GetBoolean();
                    } if (japMatchPlayer["is_goalkeeper"].ValueType.Equals(JsonValueType.Boolean))
                    {
                        _isGoalkeeper = japMatchPlayer["is_goalkeeper"].GetBoolean();
                    }

                    MatchPlayer mp = new MatchPlayer(_id, _match_id, _team_id, _player_id);
                    mp.TeamsheetValue = _teamsheet;
                    mp.Is_capitanValue = _isCaptain;
                    mp.Is_goalkeeperValue = _isGoalkeeper;
                    match.matchPlayers.Add(mp);
                }
            }
            //}
            //catch (Exception ex)
            //{

            //}
            return responseMatchPlayers.StatusCode;

        }
        private async Task<HttpStatusCode> GetYellowCards(int matchId, FootballMatch match)
        {
            //•GET /match/<id>/yellow_card
            string urlMatchPlayers = DataSource.host + "match/";
            Uri uriMatchPlayers = new Uri(urlMatchPlayers + match.id.ToString() + "/yellow_card?access_token=" + DataSource.accessToken + "&" + Guid.NewGuid().ToString());
            var responseMatchPlayers = await httpClient.GetAsync(uriMatchPlayers);
            //try
            //{
            responseMatchPlayers.EnsureSuccessStatusCode();
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            string resultMatchPlayers = await responseMatchPlayers.Content.ReadAsStringAsync();
            JsonObject jsonObjectMatchPlayers = JsonObject.Parse(resultMatchPlayers);
            //try
            //{
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
                    int _player_id = 0;
                    int _minute = 0;
                    int _addition_minute = 0;
                    string _note = "";
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
                    if (japMatchPlayer["player_id"].ValueType.Equals(JsonValueType.Number))
                    {
                        _player_id = (int)japMatchPlayer["player_id"].GetNumber();
                    }
                    if (japMatchPlayer["minute"].ValueType.Equals(JsonValueType.Number))
                    {
                        _minute = (int)japMatchPlayer["minute"].GetNumber();
                    }
                    if (japMatchPlayer["addition_minute"].ValueType.Equals(JsonValueType.Number))
                    {
                        _addition_minute = (int)japMatchPlayer["addition_minute"].GetNumber();
                    }
                    if (japMatchPlayer["note"].ValueType.Equals(JsonValueType.String))
                    {
                        _note = japMatchPlayer["note"].GetString();
                    }

                    YellowCard yc = new YellowCard(_id, _match_id, _team_id, _player_id, _minute, _addition_minute, _note);
                    match.yellowcards.Add(yc);
                }
            }
            //}
            //catch (Exception ex)
            //{

            //}
            return responseMatchPlayers.StatusCode;

        }
        private async Task<HttpStatusCode> GetRedCards(int matchId, FootballMatch match)
        {
            //•GET /match/<id>/yellow_card
            string urlRCPlayers = DataSource.host + "match/";
            Uri uriRCPlayers = new Uri(urlRCPlayers + match.id.ToString() + "/red_card?access_token=" + DataSource.accessToken + "&" + Guid.NewGuid().ToString());

            var responseRCPlayers = await httpClient.GetAsync(uriRCPlayers);
            //try
            //{
            responseRCPlayers.EnsureSuccessStatusCode();
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            string resultRCPlayers = await responseRCPlayers.Content.ReadAsStringAsync();
            JsonObject jsonObjectRCPlayers = JsonObject.Parse(resultRCPlayers);
            //try
            //{
            JsonArray jap = jsonObjectRCPlayers["rows"].GetArray();
            if (jap.Count > 0)
            {
                foreach (JsonValue jsonValueApplicant in jap)
                {
                    bool b = true;
                    JsonObject japMatchPlayer = jsonValueApplicant.GetObject();
                    int _id = 0;
                    int _match_id = 0;
                    int _team_id = 0;
                    int _player_id = 0;
                    int _minute = 0;
                    int _addition_minute = 0;
                    bool _is_two_yellow = false;
                    string _note = "";
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
                    if (japMatchPlayer["player_id"].ValueType.Equals(JsonValueType.Number))
                    {
                        _player_id = (int)japMatchPlayer["player_id"].GetNumber();
                    }
                    if (japMatchPlayer["is_two_yellow"].ValueType.Equals(JsonValueType.Boolean))
                    {
                        _is_two_yellow = japMatchPlayer["is_two_yellow"].GetBoolean();
                    }
                    if (japMatchPlayer["minute"].ValueType.Equals(JsonValueType.Number))
                    {
                        _minute = (int)japMatchPlayer["minute"].GetNumber();
                    }
                    if (japMatchPlayer["addition_minute"].ValueType.Equals(JsonValueType.Number))
                    {
                        _addition_minute = (int)japMatchPlayer["addition_minute"].GetNumber();
                    }
                    if (japMatchPlayer["note"].ValueType.Equals(JsonValueType.String))
                    {
                        _note = japMatchPlayer["note"].GetString();
                    }

                    RedCard yc = new RedCard(_id, _match_id, _team_id, _player_id, _is_two_yellow, _minute, _addition_minute, _note);
                    match.redcards.Add(yc);
                }
            }
            //}
            //catch (Exception ex)
            //{

            //}
            return responseRCPlayers.StatusCode;

        }
        private async Task<HttpStatusCode> GetGoals(int matchId, FootballMatch match)
        {
            //•GET /match/<id>/yellow_card
            string urlRCPlayers = DataSource.host + "match/";
            Uri uriRCPlayers = new Uri(urlRCPlayers + match.id.ToString() + "/goal?access_token=" + DataSource.accessToken + "&" + Guid.NewGuid().ToString());

            var responseGoals = await httpClient.GetAsync(uriRCPlayers);
            //try
            //{
            responseGoals.EnsureSuccessStatusCode();
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            string resultGoals = await responseGoals.Content.ReadAsStringAsync();
            JsonObject jsonObjectGoals = JsonObject.Parse(resultGoals);

            //try
            //{
            JsonArray jap = jsonObjectGoals["rows"].GetArray();
            if (jap.Count > 0)
            {
                foreach (JsonValue jsonValueApplicant in jap)
                {
                    bool b = true;
                    JsonObject japMatchPlayer = jsonValueApplicant.GetObject();
                    int _id = 0;
                    int _match_id = 0;
                    int _team_id = 0;
                    int _player_id = 0;
                    int _minute = 0;
                    int _addition_minute = 0;
                    int _assistant_id = 0;
                    bool _is_penalty = false;
                    bool _is_autogoal = false;
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
                    if (japMatchPlayer["player_id"].ValueType.Equals(JsonValueType.Number))
                    {
                        _player_id = (int)japMatchPlayer["player_id"].GetNumber();
                    }
                    if (japMatchPlayer["assistant_id"].ValueType.Equals(JsonValueType.Number))
                    {
                        _assistant_id = (int)japMatchPlayer["assistant_id"].GetNumber();
                    }
                    if (japMatchPlayer["is_penalty"].ValueType.Equals(JsonValueType.Boolean))
                    {
                        _is_penalty = japMatchPlayer["is_penalty"].GetBoolean();
                    }
                    if (japMatchPlayer["is_autogoal"].ValueType.Equals(JsonValueType.Boolean))
                    {
                        _is_autogoal = japMatchPlayer["is_autogoal"].GetBoolean();
                    }
                    if (japMatchPlayer["minute"].ValueType.Equals(JsonValueType.Number))
                    {
                        _minute = (int)japMatchPlayer["minute"].GetNumber();
                    }
                    if (japMatchPlayer["addition_minute"].ValueType.Equals(JsonValueType.Number))
                    {
                        _addition_minute = (int)japMatchPlayer["addition_minute"].GetNumber();
                    }


                    Goal yc = new Goal(_id, _match_id, _team_id, _player_id, _assistant_id, _is_autogoal, _is_penalty, _minute, _addition_minute);
                    match.goals.Add(yc);
                }
            }
            //}
            //catch (Exception ex)
            //{

            //}
            return responseGoals.StatusCode;

        }
        private async Task<bool> GetApplicant(FootballMatch match, int tournamentId)
        {
            //api.sportand.me:8000/team/16429/applicant?access_token=xxx&tournament_id=1728
            string urlApplicant = DataSource.host + "team/";
            Uri uriApplicantTeam1 = new Uri(urlApplicant + match.team1.id.ToString() + "/applicant?access_token=" + DataSource.accessToken + "&tournament_id=" + tournamentId.ToString() + "&" + Guid.NewGuid().ToString());
            Uri uriApplicantTeam2 = new Uri(urlApplicant + match.team2.id.ToString() + "/applicant?access_token=" + DataSource.accessToken + "&tournament_id=" + tournamentId.ToString() + "&" + Guid.NewGuid().ToString());
            var responseApplicantTeam1 = await httpClient.GetAsync(uriApplicantTeam1);
            //try
            //{
            responseApplicantTeam1.EnsureSuccessStatusCode();
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            string resultApplicantTeam1 = await responseApplicantTeam1.Content.ReadAsStringAsync();
            JsonObject jsonObjectApplicantTeam1 = JsonObject.Parse(resultApplicantTeam1);

            var responseApplicantTeam2 = await httpClient.GetAsync(uriApplicantTeam2);
            
            //try
            //{
            responseApplicantTeam2.EnsureSuccessStatusCode();
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            string resultApplicantTeam2 = await responseApplicantTeam2.Content.ReadAsStringAsync();
            JsonObject jsonObjectApplicantTeam2 = JsonObject.Parse(resultApplicantTeam2);
            //try
            //{
            JsonObject[] ja = new JsonObject[2];
            ja[0] = (jsonObjectApplicantTeam1);
            ja[1] = (jsonObjectApplicantTeam2);
            //JsonValue itemValue in groupObject["Items"].GetArray())
            //{
            //    JsonObject itemObject = itemValue.GetObject();
            for (int i = 0; i <= 1; i++)
            {
                JsonObject jsonObjectApplicant = ja[i];
                JsonArray jap = jsonObjectApplicant["rows"].GetArray();
                if (jap.Count > 0)
                {
                    foreach (JsonValue jsonValueApplicant in jap)
                    {
                        JsonObject rowObject = jsonValueApplicant.GetObject();
                        JsonObject japPlayer = rowObject["player"].GetObject();
                        String name = "";
                        String image_path = "";
                        String position = "";
                        int number = 0;
                        int gender = 1;
                        if (japPlayer["full_name"].ValueType.Equals(JsonValueType.String))
                        {
                            name = japPlayer["full_name"].GetString();
                        }
                        if (japPlayer["image_path"].ValueType.Equals(JsonValueType.String))
                        {
                            image_path = japPlayer["image_path"].GetString();
                        }
                        if (japPlayer["position"].ValueType.Equals(JsonValueType.String))
                        {
                            position = japPlayer["position"].GetString();
                        }
                        if (japPlayer["number"].ValueType.Equals(JsonValueType.Number))
                        {
                            number = (int)japPlayer["number"].GetNumber();
                        }
                        if (japPlayer["gender"].ValueType.Equals(JsonValueType.Number))
                        {
                            gender = (int)japPlayer["gender"].GetNumber();
                        }
                        Player p = new Player((int)japPlayer["id"].GetNumber(), name, image_path, position, gender, number);
                        if (i == 0)
                        {
                            match.team1_players.Add(p);
                        }
                        else
                        {
                            match.team2_players.Add(p);
                        }
                    }
                }
            }

            //}
            //catch (Exception ex)
            //{

            //}
            return true;
        }
        public static async Task<bool> Synch(bool SaveLocalDataOnServer)
        {
            bool result = true;
            if (!busy)
            {
                try
                {
                    if (SaveLocalDataOnServer)
                    {
                        try
                        {
                            busy = true;
                            await DataSource.RefreshAccessToken();
                            foreach (Tournament t in _DataSource._tournaments)
                            {
                                foreach (Round r in t.RoundsList)
                                {
                                    foreach (FootballMatch fm in r.Items)
                                    {

                                        if (fm.addition != null) await fm.addition.Update(fm.id);
                                        foreach (MatchPlayer mp in fm.matchPlayers)
                                        {
                                            await mp.Update();
                                        }
                                        foreach (RedCard rc in fm.redcards)
                                        {
                                            await rc.Update();
                                        }
                                        foreach (YellowCard yc in fm.yellowcards)
                                        {
                                            await yc.Update();
                                        }
                                        foreach (Goal gl in fm.goals)
                                        {
                                            await gl.Update();
                                        }
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            result = false;
                            throw new Exception(ex.Message);
                            //all unsaved data will be lost if continue
                        }
                        finally
                        {
                            busy = false;
                        }
                    }
                    try
                    {
                        _DataSource._tournaments.Clear();
                        busy = false;
                        await _DataSource.GetRefereeMatchCollectionAsync();
                        await Save();
                    }
                    catch (Exception ex)
                    {
                        
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        busy = false;
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    busy = false;
                }
            }
            
            return result;
        }
        private async Task<bool> GetProtocolItemsGroup(FootballMatch match, Tournament t)
        {
            await GetApplicant(match, t.id);
            await GetMatchPlayers(match.id, match);
            await GetYellowCards(match.id, match);
            await GetRedCards(match.id, match);
            await GetGoals(match.id, match);
            return true;
        }
        private async Task<bool> GetItemsForMatch(FootballMatch match)
        {
            string UrlRound = DataSource.host + "round/";
            string UrlTournament = DataSource.host + "tournament/";
            Tournament t;
            Round r;

            Uri UriRound = new Uri(UrlRound + match.round_id.ToString() + "?access_token=" + DataSource.accessToken + "&" + Guid.NewGuid().ToString());
            //StorageFile fileRound = await StorageFile.GetFileFromApplicationUriAsync(UriRound);
            var responseR = await httpClient.GetAsync(UriRound);
            //try
            //{
            responseR.EnsureSuccessStatusCode();
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            string resultR = await responseR.Content.ReadAsStringAsync();
            JsonObject jsonObjectRound = JsonObject.Parse(resultR);


            Uri UriTrnmt = new Uri(UrlTournament + jsonObjectRound["tournament_id"].GetNumber().ToString() + "?access_token=" + DataSource.accessToken + "&" + Guid.NewGuid().ToString());
            var responseT = await httpClient.GetAsync(UriTrnmt);
            //try
            //{
            responseT.EnsureSuccessStatusCode();
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            string resultT = await responseT.Content.ReadAsStringAsync();
            JsonObject jsonObjectTournament = JsonObject.Parse(resultT);
            //StorageFile fileTournament = await StorageFile.GetFileFromApplicationUriAsync(UriTrnmt);
            //string jsonTextTournament = await FileIO.ReadTextAsync(fileTournament);
            //JsonObject jsonObjectTournament = JsonObject.Parse(jsonTextTournament);
            string impath = "";
            if (jsonObjectTournament["image_path"].ValueType.Equals(JsonValueType.String)) impath = jsonObjectTournament["image_path"].GetString();
            t = new Tournament((int)jsonObjectTournament["id"].GetNumber(), jsonObjectTournament["title"].GetString(), jsonObjectTournament["location"].GetString(), impath);
            r = new Round((int)jsonObjectRound["id"].GetNumber(), jsonObjectRound["name"].GetString(), (int)jsonObjectRound["tournament_id"].GetNumber());

            await GetProtocolItemsGroup(match, t);
            Tournament to = await GetTournamentAsync((int)jsonObjectRound["tournament_id"].GetNumber());
            Round ro = await GetRoundAsync(match.round_id);
            if (to == null)
            {

                r.Items.Add(match);
                t.RoundsList.Add(r);
                this.Tournaments.Add(t);
            }
            else if (ro == null)
            {
                r.Items.Add(match);
                to.RoundsList.Add(r);
            }
            else if (await GetItemAsync(match.id) == null)
            {
                ro.Items.Add(match);
            }
            return true;
        }

        public async static void logout()
        {
            await ClearSave();
            _DataSource._tournaments = null;
            accessToken = "";
            _DataSource._tournaments = new ObservableCollection<Tournament>();
            DataSource.busy = false;
        }

        private async Task GetRefereeMatchCollectionAsync()
        {
            if (this._tournaments.Count != 0)
            {
                return;
            }
            if (DataSource.busy == true)
            {
                return;
            }
            try
            {
                busy = true;
                Uri dataUri = new Uri(host + "referee/me/match?access_token=" + accessToken + "&filter=future");
                var responseR = await httpClient.GetAsync(dataUri);
                try
                {
                    responseR.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    if (responseR.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        logout();
                    }
                    //
                    //Возможно, следует обрабатывать исключение здесь и возвращать HttpStatusCode здесь и во всех подобных запросах, которые сейчас кидают Exception
                    //
                    throw new Exception(ex.Message);
                    return;
                }
                JsonArray jsonArray;
                string resultR = await responseR.Content.ReadAsStringAsync();
                JsonObject jsonObject = JsonObject.Parse(resultR);
                if (jsonObject.ContainsKey("rows"))
                {
                    jsonArray = jsonObject["rows"].GetArray();
                    if (jsonArray.Count > 0)
                    {
                        foreach (JsonValue itemValue in jsonArray)
                        {
                            JsonObject itemObject = itemValue.GetObject();
                            //JsonObject groupObject = jsonArray[0].GetObject();
                            FootballMatch item = new FootballMatch((int)itemObject["id"].GetNumber(), true);
                            if (itemObject["start_at"].ValueType.Equals(JsonValueType.String)) item.start_at = itemObject["start_at"].GetString();
                            if (itemObject["place"].ValueType.Equals(JsonValueType.String)) item.place = itemObject["place"].GetString();
                            if (itemObject["goals1"].ValueType.Equals(JsonValueType.Number)) item.goals1 = (int)itemObject["goals1"].GetNumber();
                            if (itemObject["goals2"].ValueType.Equals(JsonValueType.Number)) item.goals2 = (int)itemObject["goals2"].GetNumber();
                            if (itemObject["penalty1"].ValueType.Equals(JsonValueType.Number)) item.penalty1 = (int)itemObject["penalty1"].GetNumber();
                            if (itemObject["penalty2"].ValueType.Equals(JsonValueType.Number)) item.penalty2 = (int)itemObject["penalty2"].GetNumber();
                            if (itemObject["round_id"].ValueType.Equals(JsonValueType.Number)) item.round_id = (int)itemObject["round_id"].GetNumber();
                            if (itemObject["is_technical"].ValueType.Equals(JsonValueType.Boolean)) item.is_technical = (bool)itemObject["is_technical"].GetBoolean();
                            if (itemObject["is_overtime"].ValueType.Equals(JsonValueType.Boolean)) item.is_overtime = (bool)itemObject["is_overtime"].GetBoolean();
                            if (itemObject["is_shootout"].ValueType.Equals(JsonValueType.Boolean)) item.is_shootout = (bool)itemObject["is_shootout"].GetBoolean();
                            JsonObject jsonObjectTeam = itemObject["team1"].GetObject();
                            FootballTeam _team1 = new FootballTeam((int)(jsonObjectTeam["id"].GetNumber()));
                            _team1.title = jsonObjectTeam["title"].GetString();
                            if (jsonObjectTeam["image_path"].ValueType.Equals(JsonValueType.String)) _team1.image_path = jsonObjectTeam["image_path"].GetString();
                            else _team1.image_path = "ms-appx:///Assets/noIm.png";
                            item.team1 = _team1;
                            jsonObjectTeam = itemObject["team2"].GetObject();
                            FootballTeam _team2 = new FootballTeam((int)(jsonObjectTeam["id"].GetNumber()));
                            _team2.title = jsonObjectTeam["title"].GetString();
                            if (jsonObjectTeam["image_path"].ValueType.Equals(JsonValueType.String)) _team2.image_path = jsonObjectTeam["image_path"].GetString();
                            else _team2.image_path = "ms-appx:///Assets/noIm.png";
                            item.team2 = _team2;
                            Addition ad = new Addition(0);
                            bool b = await ad.Get(item.id);
                            if (b)
                            {
                                item.addition = ad;
                            }
                            b = false;

                            try { await GetItemsForMatch(item); }
                            catch (Exception ex)
                            {
                                busy = false;
                                throw new Exception("Error aquired while loading data " + ex.Message);
                            }
                        }
                    }
                }
                //this.Groups.Add(actual);

                dataUri = new Uri(host + "referee/me/match?access_token=" + accessToken + "&filter=past");
                var responseL = await httpClient.GetAsync(dataUri);
                try
                {
                    responseL.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    busy = false;
                    return;
                }
                string resultL = await responseL.Content.ReadAsStringAsync();
                JsonObject jsonObjectL = JsonObject.Parse(resultL);
                if (jsonObjectL.ContainsKey("rows"))
                {
                    jsonArray = jsonObjectL["rows"].GetArray();
                    if (jsonArray.Count > 0)
                    {
                        //DataGroup past = new DataGroup(false);
                        foreach (JsonValue itemValue in jsonArray)
                        {
                            JsonObject itemObject = itemValue.GetObject();
                            FootballMatch item = new FootballMatch(((int)itemObject["id"].GetNumber()), false);
                            if (itemObject["start_at"].ValueType.Equals(JsonValueType.String)) item.start_at = itemObject["start_at"].GetString();
                            if (itemObject["place"].ValueType.Equals(JsonValueType.String)) item.place = itemObject["place"].GetString();
                            if (itemObject["goals1"].ValueType.Equals(JsonValueType.Number)) item.goals1 = (int)itemObject["goals1"].GetNumber();
                            if (itemObject["goals2"].ValueType.Equals(JsonValueType.Number)) item.goals2 = (int)itemObject["goals2"].GetNumber();
                            if (itemObject["penalty1"].ValueType.Equals(JsonValueType.Number)) item.penalty1 = (int)itemObject["penalty1"].GetNumber();
                            if (itemObject["penalty2"].ValueType.Equals(JsonValueType.Number)) item.penalty2 = (int)itemObject["penalty2"].GetNumber();
                            if (itemObject["round_id"].ValueType.Equals(JsonValueType.Number)) item.round_id = (int)itemObject["round_id"].GetNumber();
                            if (itemObject["is_technical"].ValueType.Equals(JsonValueType.Boolean)) item.is_technical = (bool)itemObject["is_technical"].GetBoolean();
                            if (itemObject["is_overtime"].ValueType.Equals(JsonValueType.Boolean)) item.is_overtime = (bool)itemObject["is_overtime"].GetBoolean();
                            if (itemObject["is_shootout"].ValueType.Equals(JsonValueType.Boolean)) item.is_shootout = (bool)itemObject["is_shootout"].GetBoolean();
                            JsonObject jsonObjectTeam = itemObject["team1"].GetObject();
                            FootballTeam _team1 = new FootballTeam((int)(jsonObjectTeam["id"].GetNumber()));
                            _team1.title = jsonObjectTeam["title"].GetString();
                            if (jsonObjectTeam["image_path"].ValueType.Equals(JsonValueType.String)) _team1.image_path = jsonObjectTeam["image_path"].GetString();
                            else _team1.image_path = "ms-appx:///Assets/noIm.png";
                            item.team1 = _team1;
                            jsonObjectTeam = itemObject["team2"].GetObject();
                            FootballTeam _team2 = new FootballTeam((int)(jsonObjectTeam["id"].GetNumber()));
                            _team2.title = jsonObjectTeam["title"].GetString();
                            if (jsonObjectTeam["image_path"].ValueType.Equals(JsonValueType.String)) _team2.image_path = jsonObjectTeam["image_path"].GetString();
                            else _team2.image_path = "ms-appx:///Assets/noIm.png";
                            item.team2 = _team2;
                            Addition ad = new Addition(9);
                            ad.attendance = 12;
                            item.addition = ad;
                            //past.Items.Add(item);
                            try { await GetItemsForMatch(item); }
                            catch (Exception ex)
                            {
                                DataSource.busy = false;
                                throw new Exception("Error aquired while loading data " + ex.Message);
                            }
                        }
                    }
                }
                if (_DataSource._tournaments.Count > 0)
                {
                    await Save();
                }
                else
                {
                    logout();
                }
            }
            catch (Exception ex)
            {
                busy = false;
                throw new Exception("Error aquired while loading data " + ex.Message);
            }
            finally
            {
                busy = false;
            }
            
        }

        public static async Task<bool> RefreshAccessToken()
        {
            bool NeedUpdate = false;
            try{
                TimeSpan i = DateTime.Parse(DataSource.expire_at).Subtract(DateTime.Now);
                if (i < TimeSpan.Zero)
                {
                    NeedUpdate = true;
                }
            }
                catch (Exception ex)
            {
                NeedUpdate = true;
            }
            if (NeedUpdate)
            {
                HttpClient httpClient = new HttpClient();
                Uri uri = new Uri(DataSource.host + "oauth/token");
                List<KeyValuePair<string, string>> pairsToSend = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("refresh_token", DataSource.refreshToken),
                new KeyValuePair<string, string>("client_id", DataSource.clientId),
                new KeyValuePair<string, string>("client_secret", DataSource.clientSecret),
                new KeyValuePair<string, string>("redirect_uri", DataSource.redirect_uri),
                new KeyValuePair<string, string>("grant_type", "refresh_token")
            };
                var content = new FormUrlEncodedContent(pairsToSend);
                var response = await httpClient.PostAsync(uri, content);
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
                DataSource.accessToken = jsonObject["access_token"].GetString();
                DataSource.refreshToken = jsonObject["refresh_token"].GetString();
                DataSource.expire_at = jsonObject["expire_at"].GetString();
                _DataSource._accessToken = jsonObject["access_token"].GetString();
                _DataSource._refreshToken = jsonObject["refresh_token"].GetString();
                _DataSource._expire_at = jsonObject["expire_at"].GetString();
            }
            return true;

        }

        public static async Task<HttpStatusCode> GetAccessKey(String login, String password)
        {
#if DEBUG
            login = "rivannr@outlook.com";
            password = "111222";
#endif
            HttpClient httpClient = new HttpClient();
            //string host = "http://api.sportand.me:8000/oauth/direct";
            //Uri uri = new Uri(host);
            //string postData = "client_id=6&client_secret=4d3bc048b6f847ght9d5vf9fbc8q0m8x&redirect_uri=samapi://success&login=rivannr@outlook.com&pass=31ker12";
            //StringContent sc = new StringContent(postData);
            Uri uri = new Uri(DataSource.host + "oauth/direct");
            DataSource.login = login;
            List<KeyValuePair<string, string>> pairsToSend = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", DataSource.clientId),
                new KeyValuePair<string, string>("client_secret", DataSource.clientSecret),
                new KeyValuePair<string, string>("redirect_uri", DataSource.redirect_uri),
                new KeyValuePair<string, string>("login", DataSource.login),
                new KeyValuePair<string, string>("pass", password)
            };

            var content = new FormUrlEncodedContent(pairsToSend);

            var response = await httpClient.PostAsync(uri, content);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized) throw new Exception("Неверный логин/пароль");
                else if (response.StatusCode == HttpStatusCode.NotFound) throw new Exception("Ошибка соединения");
                else throw new Exception(ex.Message);
                //return false;
            }
            string result = await response.Content.ReadAsStringAsync();
            JsonObject jsonObject = JsonObject.Parse(result);
            DataSource.accessToken = jsonObject["access_token"].GetString();
            DataSource.refreshToken = jsonObject["refresh_token"].GetString();
            _DataSource._accessToken = jsonObject["access_token"].GetString();
            _DataSource._refreshToken = jsonObject["refresh_token"].GetString();
            _DataSource._expire_at = jsonObject["expire_at"].GetString();
            DataSource.expire_at = jsonObject["expire_at"].GetString();
            _DataSource._login = login;

            return response.StatusCode;

        }
        public static async Task ClearSave()
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile textFile = await localFolder.GetFileAsync("cacheData.json");
                await textFile.DeleteAsync();
                //string s = await WriteTextFile("cacheData.json", "");
                //File
            }
            catch (Exception ex)
            {

            }
        }

        public static async Task Save()
        {
            string json = JsonConvert.SerializeObject(_DataSource, Formatting.Indented);
            string s = await WriteTextFile("cacheData.json", json);
        }
        public static async Task Load()
        {
            string jsonCache = await ReadTextFile("cacheData.json");
            DataSource ds = JsonConvert.DeserializeObject<DataSource>(jsonCache);
            if (ds != null) _DataSource = ds;
            DataSource.accessToken = _DataSource._accessToken;
            DataSource.refreshToken = _DataSource._refreshToken;
            DataSource.login = _DataSource._login;
            DataSource.expire_at = _DataSource._expire_at;
        }
        public static async Task<string> WriteTextFile(string filename, string contents)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile textFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            using (IRandomAccessStream textStream = await textFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (DataWriter textWriter = new DataWriter(textStream))
                {
                    textWriter.WriteString(contents);
                    await textWriter.StoreAsync();
                }
            }

            return textFile.Path;
        }
        public static async Task<string> ReadTextFile(string filename)
        {
            string contents;

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile textFile = await localFolder.GetFileAsync(filename);

            using (IRandomAccessStream textStream = await textFile.OpenReadAsync())
            {
                using (DataReader textReader = new DataReader(textStream))
                {
                    uint textLength = (uint)textStream.Size;
                    await textReader.LoadAsync(textLength);
                    contents = textReader.ReadString(textLength);
                }
            }
            return contents;
        }


    }
}
