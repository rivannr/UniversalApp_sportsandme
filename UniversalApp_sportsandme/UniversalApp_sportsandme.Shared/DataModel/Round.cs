using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using UniversalApp_sportsandme.DataModel;

namespace UniversalApp_sportsandme.DataModel
{
    public class Round
    {
        public int id { get; set; }
        public string name { get; set; }
        public int tournament_id { get; set; }

        public ObservableCollection<FootballMatch> Items { get; private set; }

        public Round(int id, String name, int tournament_id)
        {
            this.id = id;
            this.name = name;
            this.tournament_id = tournament_id;
            Items = new ObservableCollection<FootballMatch>();
        }
    }
}
