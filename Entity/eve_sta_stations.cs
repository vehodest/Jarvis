//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class eve_sta_stations
    {
        public int station_id { get; set; }
        public int solarsystem_id { get; set; }
        public string solarsystem_name { get; set; }
        public int region_id { get; set; }
        public string region_name { get; set; }
        public int station_type_id { get; set; }
        public string station_name { get; set; }
        public int corporation_id { get; set; }
        public string corporation_name { get; set; }
        public System.DateTime created { get; set; }
        public System.DateTime updated { get; set; }
    }
}
