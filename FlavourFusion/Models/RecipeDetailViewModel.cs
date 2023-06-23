using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlavourFusion.Models
{
    public class RecipeDetailViewModel
    {
        public Tbl_Recipe Recipe { get; set; }
        public List<Tbl_Recipe_Category> Categories { get; set; }
        public string recipe_description { get; set; }
        public int recipe_duration { get; set; }
        public string recipe_image { get; set; }
        public string recipe_ingredients { get; set; }
        public string recipe_name { get; set; }
        public DateTime recipe_publish_date { get; set; }
        public int recipe_serving_people { get; set; }
        public string recipe_tags { get; set; }

    }
}