//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FlavourFusion.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tbl_Replies
    {
        public int reply_id { get; set; }
        public int comment_id { get; set; }
        public int user_id { get; set; }
        public string reply_text { get; set; }
        public System.DateTime reply_date { get; set; }
    
        public virtual Tbl_Comments Tbl_Comments { get; set; }
        public virtual Tbl_Users Tbl_Users { get; set; }
    }
}
