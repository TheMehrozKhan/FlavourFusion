using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlavourFusion.Models
{
    public class ContestViewModel
    {
        public Tbl_Contests Contest { get; set; }
        public Tbl_Submissions Submission { get; set; }
    }
}