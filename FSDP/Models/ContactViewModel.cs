using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSDP.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "This Field is Required")]
        public string Name { get; set; }

        public string Subject { get; set; }

        [Required(ErrorMessage = "This Field is Required")]
        [EmailAddress(ErrorMessage = "This Field is Required")]
        [Display(Name = "Your Email Address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "An error has occured. If it happens agian please contact and administrator.")]
        [UIHint("MultilineText")]
        public string Message { get; set; }
    }
}