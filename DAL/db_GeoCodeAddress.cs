//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    
    public partial class db_GeoCodeAddress
    {
        public System.Guid Id { get; set; }
        public string formatted_address { get; set; }
        public string location_type { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
    }
}
