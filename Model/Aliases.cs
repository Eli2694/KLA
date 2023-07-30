using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Aliases
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string ID { get; set; }

        [Column(Order = 1)]
        [StringLength(256)]
        public string PreviousAliasName { get; set; }

        [Key]
        [StringLength(256)]
        public string CurrentAliasName { get; set; }

        [Key]
        [StringLength(255)]
        public string Scope { get; set; }

        public DateTime? AliasCreated { get; set; }

        [ForeignKey("Scope,ID")]
        public UniqueIds UniqueId { get; set; }

    }

}
