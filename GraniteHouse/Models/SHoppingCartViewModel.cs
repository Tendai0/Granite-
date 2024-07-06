using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Models
{
    public class SHoppingCartViewModel
    {
        public ICollection<Products> Products { get; set; }
        public Appointments Appointments { get; set; }
    }
}
