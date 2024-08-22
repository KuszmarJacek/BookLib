using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLib.Entities
{
    public class BookRating
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        [Range(1, 5)]
        public required int Score { get; set; }
        public string? Comment { get; set; }
    }
}
