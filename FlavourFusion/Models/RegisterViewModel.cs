using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlavourFusion.Models
{
    public class RegisterViewModel
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string CreditCardNumber { get; set; }
        // Plan details
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public int PlanPrice { get; set; }
        public int PlanDuration { get; set; }
    }

}