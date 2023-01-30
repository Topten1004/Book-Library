using Book.Library.Data;
using Book.Library.Data.Entities;
using Newtonsoft.Json;

namespace Book.Library.Api.Utils
{
    public class JsonFileHelper
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public JsonFileHelper(IWebHostEnvironment env, ApplicationDbContext context)
        {
            _env = env;
            _context = context;
        }

        public async Task SeedDatabase()
        {
            // Read the JSON file
            var filePath = Path.Combine(_env.ContentRootPath, "Data/books.json");
            var json = await File.ReadAllTextAsync(filePath);

            // Deserialize the JSON data into a list of objects
            var seedData = JsonConvert.DeserializeObject<List<BookEntity>>(json);

            // Add the data to the database
            _context.Books.AddRange(seedData);
            await _context.SaveChangesAsync();
        }

    }
}
