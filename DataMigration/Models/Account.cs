using System;
using System.Collections.Generic;

namespace DataMigration.Models
{
    public partial class Account
    {
        public int Id { get; set; }
        public int UserIdId { get; set; }
        public string Settings { get; set; }
        public string Hid { get; set; }

        public virtual User UserId { get; set; }
    }
}
