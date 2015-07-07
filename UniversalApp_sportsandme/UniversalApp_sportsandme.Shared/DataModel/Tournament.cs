using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using UniversalApp_sportsandme.DataModel;

namespace UniversalApp_sportsandme.DataModel
{
    public class Tournament
    {
        public int id { get; set; }
        public string title { get; set; }
        public string location { get; set; }
        public string image_path { get; set; }

        public ObservableCollection<Round> RoundsList { get; set; }

        public Tournament(int id, String title, String location, String image_path)
        {
            this.id = id;
            this.title = title;
            this.location = location;
            this.image_path = image_path;
            RoundsList = new ObservableCollection<Round>();
        }
    }
}
