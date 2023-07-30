using System.ComponentModel.DataAnnotations;

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
