//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FYPAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OWNERFAVORITE
    {
        public int fav_id { get; set; }
        public Nullable<int> userid { get; set; }
        public Nullable<int> oid { get; set; }


        [NotMapped]
        public string Oname { get; set; }


        [NotMapped]
        public string Contact { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Address { get; set; }

        [NotMapped]
        public string City { get; set; }
    }
}
