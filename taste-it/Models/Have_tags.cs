//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace taste_it.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Have_tags
    {
        public int id { get; set; }
        public int id_t { get; set; }
        public int id_r { get; set; }
    
        public virtual Recipe Recipe { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
