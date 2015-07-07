using System;
using System.Collections.Generic;
using System.Text;

namespace UniversalApp_sportsandme.DataModel
{
    public class Injury
    {

        public int id { get; set; }
        public int match_id { get; set; }
        public string note { get; set; }

        public bool deleted { get; set; }

        public Injury(int id)
        {
            this.id = id;
            deleted = false;
        }

    }
}
