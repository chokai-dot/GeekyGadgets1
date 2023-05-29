using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekyGadgets.Domain.Entity
{
    public class Order
    {
        public int Id { get; set; }
        public int? SmartphoneId { get; set; }
        public DateTime DateCreated { get; set; }
        public string Address { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public int BasketId { get; set; }
        public virtual Basket Basket { get; set; }
    }
}
