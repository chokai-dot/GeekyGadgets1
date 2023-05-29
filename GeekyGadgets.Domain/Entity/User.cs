using GeekyGadgets.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace GeekyGadgets.Domain.Entity
{
    public class User
    {

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        [JsonIgnore]
        public string Password { get; set; }

        public Role Role { get; set; } = Role.User;

        public Profile Profile { get; set; }

        public Basket Basket { get; set; }
    }
}
