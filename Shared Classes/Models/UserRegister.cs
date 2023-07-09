using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared_Classes.Models
{
    public class UserRegister
    {
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
