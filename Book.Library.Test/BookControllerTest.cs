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

namespace Book.Library.Test
{
    public class BookControllerTest : IDisposable
    {
        Mock<IGenericRepository> _repository;
        Mock<IGenericService> _service;

        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:7193/") };
        public static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        public const string _jsonMediaType = "application/json";

        private readonly ITestOutputHelper output;
        public List<BookEntity> seedDatas = new List<BookEntity>();

        string json = "[\r\n\t{\r\n\t\t\"id\": \"B1\",\r\n\t\t\"author\": \"Kutner, Joe\",\r\n\t\t\"title\": \"Deploying with JRuby\",\r\n\t\t\"genre\": \"Computer\",\r\n\t\t\"price\": \"33.00\",\r\n\t\t\"publish_date\": \"2012-08-15\",\r\n\t\t\"description\": \"Deploying with JRuby is the missing link between enjoying JRuby and using it in the real world to build high-performance, scalable applications.\"\r\n\t},\r\n\t{\r\n\t\t\"id\": \"B2\",\r\n\t\t\"author\": \"Ralls, Kim\",\r\n\t\t\"title\": \"Midnight Rain\",\r\n\t\t\"genre\": \"Fantasy\",\r\n\t\t\"price\": \"5.95\",\r\n\t\t\"publish_date\": \"2000-12-16\",\r\n\t\t\"description\": \"A former architect battles corporate zombies, an evil sorceress, and her own childhood to become queen of the world.\"\r\n\t},\r\n\t{\r\n\t\t\"id\": \"B3\",\r\n\t\t\"author\": \"Corets, Eva\",\r\n\t\t\"title\": \"Maeve Ascendant\",\r\n\t\t\"genre\": \"Fantasy\",\r\n\t\t\"price\": \"5.95\",\r\n\t\t\"publish_date\": \"2000-11-17\",\r\n\t\t\"description\": \"After the collapse of a nanotechnology society in England, the young survivors lay the foundation for a new society.\"\r\n\t},\r\n\t{\r\n\t\t\"id\": \"B4\",\r\n\t\t\"author\": \"Corets, Eva\",\r\n\t\t\"title\": \"Oberon's Legacy\",\r\n\t\t\"genre\": \"Fantasy\",\r\n\t\t\"price\": \"5.95\",\r\n\t\t\"publish_date\": \"2001-03-10\",\r\n\t\t\"description\": \"In post-apocalypse England, the mysterious agent known only as Oberon helps to create a new life for the inhabitants of London. Sequel to Maeve Ascendant.\"\r\n\t},\r\n\t{\r\n\t\t\"id\": \"B5\",\r\n\t\t\"author\": \"Tolkien, JRR\",\r\n\t\t\"title\": \"The Hobbit\",\r\n\t\t\"genre\": \"Fantasy\",\r\n\t\t\"price\": \"11.95\",\r\n\t\t\"publish_date\": \"1985-09-10\",\r\n\t\t\"description\": \"If you care for journeys there and back, out of the comfortable Western world, over the edge of the Wild, and home again, and can take an interest in a humble hero blessed with a little wisdom and a little courage and considerable good luck, here is a record of such a journey and such a traveler.\"\r\n\t},\r\n\t{\r\n\t\t\"id\": \"B6\",\r\n\t\t\"author\": \"Randall, Cynthia\",\r\n\t\t\"title\": \"Lover Birds\",\r\n\t\t\"genre\": \"Romance\",\r\n\t\t\"price\": \"4.95\",\r\n\t\t\"publish_date\": \"2000-09-02\",\r\n\t\t\"description\": \"When Carla meets Paul at an ornithology conference, tempers fly as feathers get ruffled.\"\r\n\t},\r\n\t{\r\n\t\t\"id\": \"B7\",\r\n\t\t\"author\": \"Thurman, Paula\",\r\n\t\t\"title\": \"Splish Splash\",\r\n\t\t\"genre\": \"Romance\",\r\n\t\t\"price\": \"4.95\",\r\n\t\t\"publish_date\": \"2000-11-02\",\r\n\t\t\"description\": \"A deep sea diver finds true love twenty thousand leagues beneath the sea.\"\r\n\t},\r\n\t{\r\n\t\t\"id\": \"B8\",\r\n\t\t\"author\": \"Knorr, Stefan\",\r\n\t\t\"title\": \"Creepy Crawlies\",\r\n\t\t\"genre\": \"Horror\",\r\n\t\t\"price\": \"4.95\",\r\n\t\t\"publish_date\": \"2000-12-06\",\r\n\t\t\"description\": \"An anthology of horror stories about roaches, centipedes, scorpions  and other insects.\"\r\n\t},\r\n\t{\r\n\t\t\"id\": \"B9\",\r\n\t\t\"author\": \"Kress, Peter\",\r\n\t\t\"title\": \"Paradox Lost\",\r\n\t\t\"genre\": \"Science Fiction\",\r\n\t\t\"price\": \"6.95\",\r\n\t\t\"publish_date\": \"2000-11-02\",\r\n\t\t\"description\": \"After an inadvertant trip through a Heisenberg Uncertainty Device, James Salway discovers the problems of being quantum.\"\r\n\t},\r\n\t{\r\n\t\t\"id\": \"B10\",\r\n\t\t\"author\": \"O'Brien, Tim\",\r\n\t\t\"title\": \"Microsoft .NET: The Programming Bible\",\r\n\t\t\"genre\": \"Computer\",\r\n\t\t\"price\": \"36.95\",\r\n\t\t\"publish_date\": \"2000-12-09\",\r\n\t\t\"description\": \"Microsoft's .NET initiative is explored in detail in this deep programmer's reference.\"\r\n\t},\r\n\t{\r\n\t\t\"id\": \"B11\",\r\n\t\t\"author\": \"Sydik, Jeremy J\",\r\n\t\t\"title\": \"Design Accessible Web SiTest\",\r\n\t\t\"genre\": \"Computer\",\r\n\t\t\"price\": \"34.95\",\r\n\t\t\"publish_date\": \"2007-12-01\",\r\n\t\t\"description\": \"Accessibility has a reputation of being dull, dry, and unfriendly toward graphic design. But there is a better way: well-styled semantic markup that lets you provide the best possible results for all of your users. This book will help you provide images, video, Flash and PDF in an accessible way that looks great to your sighted users, but is still accessible to all users.\"\r\n\t},\r\n\t{\r\n\t\t\"id\": \"B12\",\r\n\t\t\"author\": \"Russell, Alex\",\r\n\t\t\"title\": \"Mastering Dojo\",\r\n\t\t\"genre\": \"Computer\",\r\n\t\t\"price\": \"38.95\",\r\n\t\t\"publish_date\": \"2008-06-01\",\r\n\t\t\"description\": \"The last couple of years have seen big changes in server-side web programming. Now it’s the client’s turn; Dojo is the toolkit to make it happen and Mastering Dojo shows you how.\"\r\n\t},\r\n\t{\r\n\t\t\"id\": \"B13\",\r\n\t\t\"author\": \"Copeland, David Bryant\",\r\n\t\t\"title\": \"Build Awesome Command-Line Applications in Ruby\",\r\n\t\t\"genre\": \"Computer\",\r\n\t\t\"price\": \"20.00\",\r\n\t\t\"publish_date\": \"2012-03-01\",\r\n\t\t\"description\": \"Speak directly to your system. With its simple commands, flags, and parameters, a well-formed command-line application is the quickest way to automate a backup, a build, or a deployment and simplify your life.\"\r\n\t}\r\n]";
        public BookControllerTest(ITestOutputHelper output)
        {
            this.output = output;
            seedDatas = JsonConvert.DeserializeObject<List<BookEntity>>(json);

            this._repository = new Mock<IGenericRepository>();
            this._service = new Mock<IGenericService>();
        }

        [Fact]
        public async void GetAllBooksOK()
        {
            ////Arrange
            var books = this.CreateBooksList();
            var userRepository = new Mock<IGenericRepository>();
            userRepository.Setup(x => x.GetBookList()).ReturnsAsync(books);
            IGenericService _service = new GenericService(userRepository.Object);

            ////Act
            List<BookEntity> result = (List<BookEntity>)_service.GetBookList().Result;

            //Assert
            Assert.Equal(true, CompareList(result, books));
            Assert.NotEmpty(result);
        }

        [Fact]
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

        //[Fact]
        //public async Task TesttPostBook()
        //{
        //    // Arrange.
        //    var expectedStatusCode = System.Net.HttpStatusCode.OK;
        //    var expectedContent = new BookEntity()
        //    {
        //        Author = "Samuel Pang",
        //        Title = "Asp.Net Core WebAPI",
        //        Genre = "Chen",
        //        Price = "26.00",
        //        Publish_Date = "2000-02-15",
        //        Description = "This is my book"
        //    };

        //    var stopwatch = Stopwatch.StartNew();

        //    // Act.
        //    var response = await _httpClient.PostAsync("api/Books", GetJsonStringContent(expectedContent));

        //    var contents = await response.Content.ReadAsStringAsync();
        //    PostItem myDeserializedClass = JsonConvert.DeserializeObject<PostItem>(contents);

        //    myDeserializedClass.value.Id = null;
        //    //Assert
        //    Assert.Equal(response.StatusCode, expectedStatusCode);
        //}

        [Fact]
        public async Task TestGetBookById()
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

        public async Task TestGetBookByAuthor()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newList = seedDatas.OrderBy(x => x.Author).ThenBy(x => Convert.ToInt32(x.Id.Remove(0, 1)));
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

        public async Task TestGetBookByTitle()
        {
            // Arrange.
            var expectedStatusCode = System.Net.HttpStatusCode.OK;

            var newList = seedDatas.OrderBy(x => x.Title);
            // Act.
            var response = await _httpClient.GetAsync("/api/Books/title");

            var contents = await response.Content.ReadAsStringAsync();
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

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
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

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
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

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
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

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
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

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
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

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
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

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
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

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
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

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
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

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
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

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
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

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
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

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
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

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
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contents);

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
        public async Task TesttUpdateBook()
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
            PostItem myDeserializedClass = JsonConvert.DeserializeObject<PostItem>(contents);

            myDeserializedClass.value.Id = null;
            //Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

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

        public static StringContent GetJsonStringContent<T>(T model)
    => new(System.Text.Json.JsonSerializer.Serialize(model), Encoding.UTF8, _jsonMediaType);

        public void Dispose()
        {
        }
    }
}
