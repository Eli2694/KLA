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
        public string ID { get; set; } // corresponds to UniqueIds.ID

        [Key]
        [Column(Order = 1)]
        [StringLength(256)]
        public string AliasName { get; set; }

        [StringLength(256)]
        public string OriginalName { get; set; } // corresponds to UniqueIds.Name

        [StringLength(255)]
        public string UniqueIdScope { get; set; } // corresponds to UniqueIds.Scope

        public DateTime? AliasCreated { get; set; }

        [ForeignKey("UniqueIdScope,OriginalName,ID")]
        public UniqueIds UniqueId { get; set; }
    }
}
