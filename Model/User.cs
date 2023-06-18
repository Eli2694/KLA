using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    
    public class User
    {
        [Key]
        [StringLength(256)]
        public string UserID { get; set; }
        public string Password { get; set; }

    }
}
