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
        public List<SubmissionViewModel> Submissions { get; set; }
        public Tbl_Users User { get; set; }
        public string user_name { get; set; }
        public string RecipeName { get; set; }
        public string RecipeDescription { get; set; }
        public string SubmissionDate { get; set; }
        public int Submission_Id { get; set; }
        public string Recipe_image { get; set; }
    }
}