using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlavourFusion.Models
{
    public class SubmissionViewModel
    {
        public Tbl_Submissions Submission { get; set; }
        public Tbl_Users User { get; set; }
        public string User_name { get; set; }
    }
}