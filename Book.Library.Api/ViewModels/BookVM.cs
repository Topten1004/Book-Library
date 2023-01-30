using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Book.Library.Api.ViewModels
{
    public class BookVM
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
