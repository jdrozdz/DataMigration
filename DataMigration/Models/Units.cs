using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigration.Models
{
    public partial class Units
    {
        public string UnitId { get; set; }
        
        public string Description { get; set; }
        
        public string Phone { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
}
