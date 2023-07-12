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
        public string AliasCurrentName { get; set; }

        [StringLength(256)]
        public string AliasPreviousName { get; set; } // corresponds to UniqueIds.Name

        [StringLength(255)]
        public string Scope { get; set; } // corresponds to UniqueIds.Scope

        public DateTime? AliasCreated { get; set; }

        [ForeignKey("Scope,OriginalName,ID")]
        public UniqueIds UniqueId { get; set; }
    }
}
