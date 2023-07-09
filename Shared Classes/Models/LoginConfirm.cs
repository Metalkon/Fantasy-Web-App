using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared_Classes.Models
{
    public class LoginConfirm
    {
        public string Code { get; set; }
        public string Username { get; set; }
        public UserLogin UserLogin { get; set; }
    }
}
