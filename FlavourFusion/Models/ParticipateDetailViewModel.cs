using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlavourFusion.Models
{
    public class ParticipateDetailViewModel
    {
        public Tbl_Recipe Recipe { get; set; }
        public Tbl_Submissions thesubmission { get; set; }
        public List<Tbl_Recipe_Category> Categories { get; set; }
        public Tbl_Users Users { get; set; }
        public string recipe_description { get; set; }
        public int recipe_duration { get; set; }
        public string recipe_image { get; set; }
        public string recipe_ingredients { get; set; }
        public string recipe_name { get; set; }
        public DateTime recipe_publish_date { get; set; }
        public int recipe_serving_people { get; set; }
        public string Recipe_tags { get; set; }
        public string recipe_tutorial_video { get; set; }
    }
}