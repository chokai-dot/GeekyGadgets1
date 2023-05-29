using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekyGadgets.Domain.Entity
{
    public class Smartphone
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string Storage { get; set; }
        public string RAM { get; set; }
        public string Display { get; set; }
        public string RearCamera { get; set; }
        public string FrontCamera { get; set; }
        public string Battery { get; set; }
        public string Processor { get; set; }
        public string OS { get; set; }
        public string Dimensions { get; set; }
        public string Weight { get; set; }
        public decimal Price { get; set; }
        public byte[]? Avatar { get; set; }
    }
}
