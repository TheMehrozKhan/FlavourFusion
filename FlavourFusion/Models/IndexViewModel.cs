using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlavourFusion.Models
{
    public class IndexViewModel
    {
        public List<Tbl_Recipe> Recipes { get; set; }
        public List<Tbl_Recipe_Category> Categories { get; set; }
        
    }
}