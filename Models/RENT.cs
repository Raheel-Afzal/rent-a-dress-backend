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
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class RENT
    {
        public int rentid { get; set; }
        public int renterid { get; set; }
        public int oid { get; set; }
        public int dressid { get; set; }
        public string rentstartdate { get; set; }
        public string rentenddate { get; set; }
        public string pickingdate { get; set; }
        public string requeststatus { get; set; }




        [NotMapped]
        public List<DRESSIMAGE> images { get; set; }

        [NotMapped]
        public string Reqname { get; set; }

        [NotMapped]
        public string Reqcontact { get; set; }

        [NotMapped]
        public string Reqaddress { get; set; }

        [NotMapped]
        public string Reqcity { get; set; }


    }
}
