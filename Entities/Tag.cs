using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLib.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<Book> Books { get; set; } = [];
    }
}
