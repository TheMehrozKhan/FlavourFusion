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
    
    public partial class Tbl_Recipe
    {
        public int recipe_id { get; set; }
        public string recipe_name { get; set; }
        public string recipe_description { get; set; }
        public string recipe_image { get; set; }
        public Nullable<int> FK_Category_Recipe { get; set; }
        public string recipe_ingredients { get; set; }
        public Nullable<int> recipe_duration { get; set; }
        public Nullable<int> recipe_serving_people { get; set; }
        public string recipe_tags { get; set; }
        public Nullable<System.DateTime> recipe_publish_date { get; set; }
    
        public virtual Tbl_Recipe_Category Tbl_Recipe_Category { get; set; }
    }
}
