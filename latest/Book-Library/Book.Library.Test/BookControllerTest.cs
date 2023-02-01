using Book.Library.Business.Services;
using Book.Library.Data.Entities;
using Book.Library.Data.Repositories;
using Castle.Core.Configuration;
using Moq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Xunit.Abstractions;

namespace Book.Library.Test
{
    public class BooksControllerTest : IDisposable
    {
        Mock<IGenericRepository> _repository;
        Mock<IGenericService> _service;

        private IConfiguration Configuration = null;

        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:7193/") };
        public static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        public const string _jsonMediaType = "application/json";

        private readonly ITestOutputHelper output;
        public List<BookEntity> seedDatas = new List<BookEntity>();

        public void Dispose() { }

        public BooksControllerTest(ITestOutputHelper output)
        {
            this.output = output;

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/Books.json");
            var json = File.ReadAllText(filePath);
            seedDatas = JsonConvert.DeserializeObject<List<BookEntity>>(json);

            this._repository = new Mock<IGenericRepository>();
            this._service = new Mock<IGenericService>();
        }

        /***/

        public class Root
        {
            public List<BookEntity> value { get; set; }
            public int statusCode { get; set; }
            public object? contentType { get; set; }
        }
        public class PostItem
        {
            public BookEntity? value { get; set; }
            public int statusCode { get; set; }
            public object? contentType { get; set; }
        }

        public bool CompareList(List<BookEntity> left, List<BookEntity> right)
        {
            if (left.Count != right.Count)
                return false;
            for (int i = 0; i < left.Count; i++)
                if (left[i].Id != right[i].Id)
                    return false;

            return true;
        }

        private BookEntity CreateBook() => new BookEntity()
        {
            Id = "B18",
            Author = "Arthur Chen",
            Title = "Asp.Net Core WebAPI",
            Genre = "Chen",
            Price = "12.00",
            Publish_Date = "2000-02-15",
            Description = "This is my book"
        };

        /***/

        #region GET SORTED

        [Fact]
        public async Task GetBookList_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newsList = seedDatas.OrderBy(x => x.Id).ToList();
            // Act.
            var response = await _httpClient.GetAsync("/api/Books");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newsList, myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSortById_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newList = seedDatas.OrderBy(x => Convert.ToInt32(x.Id.Remove(0, 1))).ToList();

            // Act.
            var response = await _httpClient.GetAsync("/api/Books/id");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSortByAuthor_Test() // PROBLEM HERE
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newList = seedDatas.OrderBy(x => x.Author);
            var stopwatch = Stopwatch.StartNew();

            // Act.
            var response = await _httpClient.GetAsync("/api/Books/author");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSortByTitle_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newList = seedDatas.OrderBy(x => x.Title);

            // Act.
            var response = await _httpClient.GetAsync("/api/Books/title");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSortByGenre_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newList = seedDatas.OrderBy(x => x.Genre).ToList();
            var stopwatch = Stopwatch.StartNew();

            // Act.
            var response = await _httpClient.GetAsync("/api/Books/genre");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSortByPrice_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newList = seedDatas.OrderBy(x => double.Parse(x.Price, CultureInfo.InvariantCulture));
            var stopwatch = Stopwatch.StartNew();

            // Act.
            var response = await _httpClient.GetAsync("/api/Books/price");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSortByPublished_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newList = seedDatas.OrderBy(x => x.Publish_Date);
            var stopwatch = Stopwatch.StartNew();

            // Act.
            var response = await _httpClient.GetAsync("/api/Books/published");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSortByDescription_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newList = seedDatas.OrderBy(x => x.Description).ToList();
            var stopwatch = Stopwatch.StartNew();
            // Act.
            var response = await _httpClient.GetAsync("/api/Books/description");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList, myDeserializedClass.value.ToList()));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        #endregion

        #region GET SEARCH PARAM

        [Fact]
        public async Task GetBooksSearchId_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            string search = "8";

            var newList = seedDatas.Where(x => x.Id.Contains(search, StringComparison.OrdinalIgnoreCase)).OrderBy(x => Convert.ToInt32(x.Id.Remove(0, 1)));
            var stopwatch = Stopwatch.StartNew();

            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/id/{search}");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSearchAuthor_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            string search = "8";

            var newList = seedDatas.Where(x => x.Author.Contains(search, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Author);
            var stopwatch = Stopwatch.StartNew();

            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/author/{search}");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSearchTitle_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            string search = "8";

            var newList = seedDatas.Where(x => x.Title.Contains(search, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Title);
            var stopwatch = Stopwatch.StartNew();
            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/title/{search}");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSearchGenre_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            string search = "Computer";

            var newList = seedDatas.Where(x => x.Genre.Contains(search, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Genre);
            var stopwatch = Stopwatch.StartNew();

            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/genre/{search}");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSearchPrice_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            string search = "8";

            var newList = seedDatas.Where(x => x.Price.Contains(search)).OrderBy(x =>
            double.Parse(x.Price, CultureInfo.InvariantCulture));

            var stopwatch = Stopwatch.StartNew();

            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/price/{search}");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSearchPrice_Min_Max_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            double minValue = 10.00;
            double maxValue = 50.00;

            var newList = seedDatas.Where(x => double.Parse(x.Price, CultureInfo.InvariantCulture)
            >= minValue && double.Parse(x.Price, CultureInfo.InvariantCulture)
            <= maxValue).OrderBy(x => double.Parse(x.Price, CultureInfo.InvariantCulture));

            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/price/{minValue}&{maxValue}");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSearchPublished_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            string search = "200";

            var newList = seedDatas.Where(x => x.Publish_Date.Contains(search)).OrderBy(x => x.Publish_Date);
            var stopwatch = Stopwatch.StartNew();
            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/published/{search}");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSearchPublishedYear_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            int year = 2000;
            string str = year.ToString();

            var newList = seedDatas.Where(x => x.Publish_Date.Contains(str)).OrderBy(x => x.Publish_Date);

            var stopwatch = Stopwatch.StartNew();

            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/published/{year}");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            foreach (var item in newList)
                output.WriteLine(item.Id);

            foreach (var item in myDeserializedClass.value)
                output.WriteLine(item.Id);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSearchPublishedMonth_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            int year = 2000;
            int month = 2;
            string str = year.ToString() + "-" + String.Format("{0:D2}", month);

            var newList = seedDatas.Where(x => x.Publish_Date.Contains(str)).OrderBy(x => x.Publish_Date);
            var stopwatch = Stopwatch.StartNew();

            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/published/{year}/{month}");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            // Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSearchPublishedDate_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            int year = 2000;
            int month = 12;
            int day = 19;

            string str = year.ToString() + "-" + String.Format("{0:D2}", month) + "-" + String.Format("{0:D2}", day);

            var newList = seedDatas.Where(x => x.Publish_Date.Contains(str)).OrderBy(x => x.Publish_Date);
            var stopwatch = Stopwatch.StartNew();

            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/published/{year}/{month}/{day}");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task GetBooksSearchDescription_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            string search = "8";

            var newList = seedDatas.Where(x => x.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Description);
            var stopwatch = Stopwatch.StartNew();

            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/description/{search}");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        #endregion

        #region POST

        [Fact]
        public async Task SaveBookDetail_Test()
        {
            // Arrange
            var book = this.CreateBook();
            var userRepository = new Mock<IGenericRepository>();
            userRepository.Setup(x => x.SaveBookDetail(book)).ReturnsAsync(book);
            IGenericService _service = new GenericService(userRepository.Object);

            // Act
            BookEntity result = _service.SaveBookDetail(book).Result;

            // Assert
            Assert.Equal(book, result);
        }

        #endregion

        #region PUT

        [Fact]
        public async Task UpdateBook_Test()
        {
            // Arrange
            var book = this.CreateBook();
            var userRepository = new Mock<IGenericRepository>();

            userRepository.Setup(x => x.UpdateBookDetail(book)).ReturnsAsync(book);
            IGenericService _service = new GenericService(userRepository.Object);

            // Act
            BookEntity result = _service.UpdateBookDetail(book).Result;

            // Assert
            Assert.Equal(book, result);
        }

        #endregion

        #region DELETE

        [Fact]
        public async Task DeleteBook_Test()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var bookId = "B19";

            // Act.
            var response = await _httpClient.DeleteAsync($"/api/Books/{bookId}");

            // Assert.
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        #endregion
    }
}
