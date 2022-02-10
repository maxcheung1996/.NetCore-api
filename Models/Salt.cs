using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public partial class Salt
    {
        [Key]
        public Guid Guid { get; set; }
        public Guid UserGuid { get; set; }
        public string? Salt1 { get; set; }

        // Navigation properties
        public virtual User user { get; set; }
    }
}
