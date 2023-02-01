using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book.Library.Data.Entities
{
    [Table("Books")]
    public class BookEntity
    {
        public string? Id { get; set; }

        public string? Author { get; set; }

        public string? Title { get; set; }

        public string? Genre { get; set; }

        public string? Price { get; set; }

        public string? Publish_Date { get; set; }

        public string? Description { get; set; }

    }
}
