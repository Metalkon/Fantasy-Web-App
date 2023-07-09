using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared_Classes.Models
{
    public class RegisterConfirm
    {
        public string Code { get; set; }
        public UserRegister UserRegister { get; set; }
    }
}
