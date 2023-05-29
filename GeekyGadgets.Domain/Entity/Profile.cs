using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekyGadgets.Domain.Entity
{
    public class Profile
    {
        public int Id { get; set; }

        public string Address { get; set; }

        public short Age { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
