//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FaceManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CheckIn
    {
        public int id { get; set; }
        public System.DateTime date { get; set; }
        public int Class_id { get; set; }
        public string Code { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Image { get; set; }
        public double Accuracy { get; set; }
    
        public virtual MyClass MyClass { get; set; }
    }
}