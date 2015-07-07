using System;
using System.Collections.Generic;
using System.Text;

namespace UniversalApp_sportsandme.DataModel
{


    public class FootballMatch
    {
        public string id { get; set; }
        public string round_id { get; set; }
        public string team1_id { get; set; }
        public string team2_id { get; set; }
        public string goals1 { get; set; }
        public string goals2 { get; set; }
        public string penalty1 { get; set; }
        public string penalty2 { get; set; }
        public string is_technical { get; set; }
        public string is_overtime { get; set; }
        public string is_shootout { get; set; }
        public string next_id { get; set; }
        public string tour_id { get; set; }
        public string tree_id { get; set; }
        public string referee { get; set; }
        public string start_at { get; set; }
        public string time_zone { get; set; }
        public string place { get; set; }

        public FootballTeam team1 { get; set; }
        public FootballTeam team2 { get; set; }
        public FootballMatch(String id,
        String round_id,
        String team1_id,
        String team2_id,
        String goals1,
        String goals2,
        String penalty1,
        String penalty2,
        String is_technical,
        String is_overtime,
        String is_shootout,
        String next_id,
        String tour_id,
        String tree_id,
        String referee,
        String start_at,
        String time_zone,
        String place)
        {
            this.id = id;
            this.round_id = round_id;
            this.team1_id = team1_id;
            this.team2_id = team2_id;
            this.goals1 = goals1;
            this.goals2 = goals2;
            this.penalty1 = penalty1;
            this.penalty2 = penalty2;
            this.is_technical = is_technical;
            this.is_overtime = is_overtime;
            this.is_shootout = is_shootout;
            this.next_id = next_id;
            this.tour_id = tour_id;
            this.tree_id = tree_id;
            this.referee = referee;
            this.start_at = start_at;
            this.time_zone = time_zone;
            this.place = place;
        }
    }

    public class FootballTeam
    {
        public string id { get; set; }
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
        public FootballTeam(String id,
                     String type_id,
                     String category_id,
                     String user_id,
                     String title,
                     String image_path,
                     String cover_photo_id,
                     String cover_photo_position,
                     String country_id,
                     String region_id,
                     String city_id,
                     String page_vk,
                     String page_fb,
                     String page_tw,
                     String page_in)
        {
            this.id = id;
            this.type_id = type_id;
            this.category_id = category_id;
            this.user_id = user_id;
            this.title = title;
            this.image_path = image_path;
            this.cover_photo_id = cover_photo_id;
            this.cover_photo_position = cover_photo_position;
            this.country_id = country_id;
            this.region_id = region_id;
            this.city_id = city_id;
            this.page_vk = page_vk;
            this.page_fb = page_fb;
            this.page_tw = page_tw;
            this.page_in = page_in;
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

    ///not finished
    public class Player
    {
        public int id { get; set; }
        public string full_name { get; set; }
        public object image_path { get; set; }
        public string position { get; set; }
        public int number { get; set; }
        public int user_id { get; set; }
        /////need more fields
        ///
    }

}
