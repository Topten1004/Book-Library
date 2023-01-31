using Book.Library.Api.Controllers;
using Book.Library.Business.Services;
using Book.Library.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Book.Library.Data.Repositories;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using Xunit.Abstractions;
using NPOI.SS.Formula.Functions;
using Newtonsoft.Json;
using Book.Library.Api.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Book.Library.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;

namespace Book.Library.Test
{
    public class BookControllerTest : IDisposable
    {
        Mock<IGenericRepository> _repository;
        Mock<IGenericService> _service;

        private IConfiguration Configuration = null;


        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:7193/") };
        public static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        public const string _jsonMediaType = "application/json";

        private readonly ITestOutputHelper output;
        public List<BookEntity> seedDatas = new List<BookEntity>();

        public BookControllerTest(ITestOutputHelper output)
        {
            this.output = output;

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/Books.json");
            var json = File.ReadAllText(filePath);
            seedDatas = JsonConvert.DeserializeObject<List<BookEntity>>(json);

            this._repository = new Mock<IGenericRepository>();
            this._service = new Mock<IGenericService>();
        }

        [Fact]
        public async Task GetAllBookItemsFromDataBase()
        {
            var str = "Server=.;Initial Catalog=BookLibDb; Integrated Security = True; trusted_connection=true;encrypt=false;";
            var context = new ApplicationDbContext(str);
            IGenericRepository _repository = new GenericRepository(context);
            var _service = new GenericService(_repository);
            List<BookEntity> sales = new List<BookEntity>();

            var bookItems = await _service.GetBookList().ConfigureAwait(false);

            Assert.True(true);
        }

        [Fact(DisplayName = "Post Request SaveBook")]
        public async void SaveBookDetail()
        {
            ////Arrange
            var book = this.CreateBook();
            var userRepository = new Mock<IGenericRepository>();
            userRepository.Setup(x => x.SaveBookDetail(book)).ReturnsAsync(book);
            IGenericService _service = new GenericService(userRepository.Object);

            ////Act
            BookEntity result = _service.SaveBookDetail(book).Result;

            //Assert
            Assert.Equal(book, result);
        }

        [Fact]
        public async void UpdateBookDetail()
        {
            ////Arrange
            var book = this.CreateBook();
            var userRepository = new Mock<IGenericRepository>();
            userRepository.Setup(x => x.UpdateBookDetail(book)).ReturnsAsync(book);
            IGenericService _service = new GenericService(userRepository.Object);

            ////Act
            BookEntity result = _service.UpdateBookDetail(book).Result;

            //Assert
            Assert.Equal(book, result);
        }
        [Fact]
        public void GetBookDetailById()
        {
            //Arrange
            var id = "B18";
            var book = this.CreateBook();
            var userRepository = new Mock<IGenericRepository>();

            userRepository.Setup(x => x.GetBookDetailById(id)).ReturnsAsync(book);
            IGenericService _service = new GenericService(userRepository.Object);

            //Act
            BookEntity result = _service.GetBookDetailById(id).Result;

            //Assert
            Assert.True(result == book);
            Assert.Equal("12.00", book.Price);
        }

        private BookEntity CreateBook() =>
           new BookEntity()
           {
              Id = "B18",
              Author = "Arthur Chen",
              Title = "Asp.Net Core WebAPI",
              Genre = "Chen",
              Price = "12.00",
              Publish_Date = "2000-02-15",
              Description = "This is my book"
           };

        private List<BookEntity> CreateBooksList()
        {
            return seedDatas;
        }

        [Fact]
        public async Task TestGetBookById()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newList = seedDatas.OrderBy(x => Convert.ToInt32(x.Id.Remove(0, 1))).ToList();
            // Act.
            var response = await _httpClient.GetAsync("/api/Books/id");

            var contents = await response.Content.ReadAsStringAsync();
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }
        [Fact]

        public async Task TestGetBookByAuthor()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newList = seedDatas.OrderBy(x => x.Author).ThenBy(x => Convert.ToInt32(x.Id.Remove(0, 1)));
            var stopwatch = Stopwatch.StartNew();
            // Act.
            var response = await _httpClient.GetAsync("/api/Books/author");

            var contents = await response.Content.ReadAsStringAsync();
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }
        [Fact]

        public async Task TestGetBookByTitle()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newList = seedDatas.OrderBy(x => x.Title);
            // Act.
            var response = await _httpClient.GetAsync("/api/Books/title");

            var contents = await response.Content.ReadAsStringAsync();
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            output.WriteLine(myDeserializedClass.value.ToString());
            output.WriteLine(myDeserializedClass.value.Count.ToString());

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }
        [Fact]
        public async Task TestGetBookByGenre()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newList = seedDatas.OrderBy(x => x.Genre).ToList();
            var stopwatch = Stopwatch.StartNew();
            // Act.
            var response = await _httpClient.GetAsync("/api/Books/genre");

            var contents = await response.Content.ReadAsStringAsync();
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }
        [Fact]
        public async Task TestGetBookByPrice()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newList = seedDatas.OrderBy(x => double.Parse(x.Price, CultureInfo.InvariantCulture));
            var stopwatch = Stopwatch.StartNew();
            // Act.
            var response = await _httpClient.GetAsync("/api/Books/price");

            var contents = await response.Content.ReadAsStringAsync();
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }
        [Fact]
        public async Task TestGetBookByPublished()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newList = seedDatas.OrderBy(x => x.Publish_Date);
            var stopwatch = Stopwatch.StartNew();
            // Act.
            var response = await _httpClient.GetAsync("/api/Books/published");

            var contents = await response.Content.ReadAsStringAsync();
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }
        [Fact]
        public async Task TestGetBookByDescription()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newList = seedDatas.OrderBy(x => x.Description).ToList();
            var stopwatch = Stopwatch.StartNew();
            // Act.
            var response = await _httpClient.GetAsync("/api/Books/description");

            var contents = await response.Content.ReadAsStringAsync();
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList, myDeserializedClass.value.ToList()));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }
        [Fact]
        public async Task TestGetBookSearchById()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            string search = "8";

            var newList = seedDatas.Where(x => x.Id.Contains(search)).OrderBy(x => Convert.ToInt32(x.Id.Remove(0, 1)));
            var stopwatch = Stopwatch.StartNew();
            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/id/{search}");

            var contents = await response.Content.ReadAsStringAsync();
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }
        [Fact]
        public async Task TestGetBookSearchByAuthor()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            string search = "8";

            var newList = seedDatas.Where(x => x.Author.Contains(search, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Author);
            var stopwatch = Stopwatch.StartNew();
            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/author/{search}");

            var contents = await response.Content.ReadAsStringAsync();
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }
        [Fact]
        public async Task TestGetBookSearchByTitle()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            string search = "8";

            var newList = seedDatas.Where(x => x.Title.Contains(search, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Title);
            var stopwatch = Stopwatch.StartNew();
            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/title/{search}");

            var contents = await response.Content.ReadAsStringAsync();
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }
        [Fact]
        public async Task TestGetBookSearchByGenre()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            string search = "8";

            var newList = seedDatas.Where(x => x.Genre.Contains(search, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Genre);
            var stopwatch = Stopwatch.StartNew();
            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/genre/{search}");

            var contents = await response.Content.ReadAsStringAsync();
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }
        [Fact]
        public async Task TestGetBookSearchByPrice()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            string search = "8";

            var newList = seedDatas.Where(x => x.Price.Contains(search)).OrderBy(x => double.Parse(x.Price, CultureInfo.InvariantCulture));
            var stopwatch = Stopwatch.StartNew();
            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/price/{search}");

            var contents = await response.Content.ReadAsStringAsync();
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }
        [Fact]
        public async Task TestGetBookSearchByPrices()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            double minValue = 10.00;
            double maxValue = 50.00;

            var newList = seedDatas.Where(x => double.Parse(x.Price, CultureInfo.InvariantCulture) >= minValue && double.Parse(x.Price, CultureInfo.InvariantCulture) <= maxValue).OrderBy(x => double.Parse(x.Price, CultureInfo.InvariantCulture));
            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/price/{minValue}&{maxValue}");

            var contents = await response.Content.ReadAsStringAsync();
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }
        [Fact]
        public async Task TestGetBookSearchByPublished()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            string search = "200";

            var newList = seedDatas.Where(x => x.Publish_Date.Contains(search)).OrderBy(x => x.Publish_Date).ToList();
            var stopwatch = Stopwatch.StartNew();
            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/published/{search}");

            var contents = await response.Content.ReadAsStringAsync();
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }
        [Fact]
        public async Task TestGetBookSearchByPublishedYear()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            int year = 2000;
            string str = year.ToString();

            var newList = seedDatas.Where(x => x.Publish_Date.Contains(str)).OrderBy(x => x.Publish_Date).ToList();

            var stopwatch = Stopwatch.StartNew();
            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/published/{year}");

            var contents = await response.Content.ReadAsStringAsync();
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            foreach (var item in newList)
                output.WriteLine(item.Id);

            foreach (var item in myDeserializedClass.value)
                output.WriteLine(item.Id);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task TestGetBookSearchByPublishedMonth()
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
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task TestGetBookSearchByPublishedDay()
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
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task TestGetBookSearchByDescription()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            string search = "8";

            var newList = seedDatas.Where(x => x.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Description);
            var stopwatch = Stopwatch.StartNew();
            // Act.
            var response = await _httpClient.GetAsync($"/api/Books/description/{search}");

            var contents = await response.Content.ReadAsStringAsync();
            GetResponseData myDeserializedClass = JsonConvert.DeserializeObject<GetResponseData>(contents);

            //Assert
            Assert.Equal(true, CompareList(newList.ToList(), myDeserializedClass.value));
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        [Fact]
        public async Task DeleteBookById()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var bookId = "B19";
            var stopwatch = Stopwatch.StartNew();

            // Act.
            var response = await _httpClient.DeleteAsync($"/api/Books/{bookId}");
            // Assert.
            Assert.Equal(response.StatusCode, expectedStatusCode);
        }

        public bool CompareList(List<BookEntity> left, List<BookEntity> right)
        {
            if(left.Count != right.Count) 
                return false;
            for (int i = 0; i < left.Count; i++)
                if (left[i].Id != right[i].Id)
                    return false;

            return true;
        }

        [Fact]
        public async Task TestUpdateBook()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var bookId = "B13";
            var expectedContent = new BookEntity()
            {
                Author = "Copeland, David Bryant",
                Title = "Build Awesome Command-Line Applications in Ruby",
                Genre = "Computer",
                Price = "20.00",
                Publish_Date = "2012-03-01",
                Description = "Speak directly to your system. With its simple commands, flags, and parameters, a well-formed command-line application is the quickest way to automate a backup, a build, or a deployment and simplify your life."
            };

            var stopwatch = Stopwatch.StartNew();

            // Act.
            var response = await _httpClient.PutAsync($"api/Books/{bookId}", GetJsonStringContent(expectedContent));

            var contents = await response.Content.ReadAsStringAsync();
            PostResponseData myDeserializedClass = JsonConvert.DeserializeObject<PostResponseData>(contents);

            myDeserializedClass.value.Id = null;
            //Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        public class GetResponseData
        {
            public List<BookEntity> value { get; set; }
            public int statusCode { get; set; }
            public object? contentType { get; set; }
        }
        public class PostResponseData
        {
            public BookEntity? value { get; set; }
            public int statusCode { get; set; }
            public object? contentType { get; set; }
        }

        public static StringContent GetJsonStringContent<T>(T model)
    => new(System.Text.Json.JsonSerializer.Serialize(model), Encoding.UTF8, _jsonMediaType);

        public void Dispose()
        {
            output.WriteLine("Dispose started");
        }
    }
}
