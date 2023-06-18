using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Aliases
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string ID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(256)]
        public string AliasName { get; set; }

        [StringLength(255)]
        public string OriginalName { get; set; }

        public DateTime? AliasCreated { get; set; }

        // Navigation property
        [ForeignKey("ID")]
        public UniqueIds UniqueId { get; set; }
    }
}
