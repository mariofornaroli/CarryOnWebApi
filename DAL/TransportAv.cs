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
    using System.Collections.Generic;
    
    public partial class TransportAv
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> AddressFrom { get; set; }
        public Nullable<System.Guid> AddreessDest { get; set; }
        public Nullable<System.DateTime> DateTransportFixed { get; set; }
        public int DateTransportType { get; set; }
        public string DateTransportInfo { get; set; }
        public int RequestState { get; set; }
        public Nullable<System.Guid> UserId { get; set; }
        public string VolAvailable { get; set; }
    
        public virtual Addresses Addresses { get; set; }
        public virtual Addresses Addresses1 { get; set; }
        public virtual CO01UT CO01UT { get; set; }
        public virtual GeoCodeAddress GeoCodeAddress { get; set; }
        public virtual GeoCodeAddress GeoCodeAddress1 { get; set; }
    }
}
