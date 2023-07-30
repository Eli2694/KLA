using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class UniqueIds
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(255)]
        public string Scope { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(256)]
        public string Name { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string ID { get; set; }

        public DateTime? Timestamp { get; set; }

        [StringLength(50)]
        public string EntityType { get; set; }

        public ICollection<Aliases> Aliases { get; set; }
    }
}
