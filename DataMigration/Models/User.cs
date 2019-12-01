using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigration.Models
{
    [Table("public.user")]
    public partial class User
    {
        public User()
        {
            UserAssociation = new HashSet<UserAssociation>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Roles { get; set; }
        public string Password { get; set; }
        public string Skill { get; set; }
        public string Displayname { get; set; }

        public virtual Account Account { get; set; }
        public virtual ICollection<UserAssociation> UserAssociation { get; set; }
    }
}
