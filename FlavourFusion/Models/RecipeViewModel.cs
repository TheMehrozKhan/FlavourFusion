using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlavourFusion.Models
{
    public class RecipeViewModel
    {
        public int recipe_id { get; set; }
        public string recipe_name { get; set; }
        public string recipe_description { get; set; }
        public string recipe_image { get; set; }
        public int category_id { get; set; }
        public string category_name { get; set; }
        public int admin_id { get; set; }
    }
}