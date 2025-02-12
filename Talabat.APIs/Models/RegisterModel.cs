﻿using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.Models
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        public string DisplayName { get; set; }


        [Required]
        [Phone]
        public string PhoneNumber { get; set; }


        [Required]
        public string Password { get; set; }
    }
}
