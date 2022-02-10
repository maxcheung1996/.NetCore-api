using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public partial class User
    {
        public User()
        {
            this.Salts = new HashSet<Salt>();
        }

        [Key]
        public Guid Guid { get; set; }
        [Required(ErrorMessage = "Eail is required")]
        public string Mail { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        public byte[] Password { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Role { get; set; } = null!;
        // Navigation property
        public virtual ICollection<Salt> Salts { get; private set; }
    }
}
