using AutoMapper;
using Book.Library.Api.ViewModels;
using Book.Library.Business.Services;
using Book.Library.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Book.Library.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IGenericService _genericService;
        private readonly IMapper _mapper;
        private readonly ILogger<BooksController> _logger;

        public BooksController(ILogger<BooksController> logger, IMapper mapper, IGenericService genericService)
        {
            _logger = logger;
            _mapper = mapper;
            _genericService = genericService;
        }

        // Method to get the list of the books
        [HttpGet]
        [Route("")]
        public async Task<IResult> GetBookList()
        {
            var result = await _genericService.GetBookList();
            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby id
        [HttpGet]
        [Route("id")]
        public async Task<IResult> GetBooksSortById()
        {
            var result = await _genericService.GetBookList();
            result = result.OrderBy(x => Convert.ToInt32(x.Id.Remove(0, 1)));

            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby author
        [HttpGet]
        [Route("author")]
        public async Task<IResult> GetBooksSortByAuthor()
        {
            var result = await _genericService.GetBookList();
            result = result.OrderBy(x => x.Author).ThenBy(x => Convert.ToInt32(x.Id.Remove(0, 1)));

            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby title
        [HttpGet]
        [Route("title")]
        public async Task<IResult> GetBooksSortByTitle()
        {
            var result = await _genericService.GetBookList();
            result = result.OrderBy(x => x.Title);

            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby genre
        [HttpGet]
        [Route("genre")]
        public async Task<IResult> GetBooksSortByGenre()
        {
            var result = await _genericService.GetBookList();
            result = result.OrderBy(x => x.Genre);

            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby price
        [HttpGet]
        [Route("price")]
        public async Task<IResult> GetBooksSortByPrice()
        {
            var result = await _genericService.GetBookList();
            result = result.OrderBy(x => double.Parse(x.Price, CultureInfo.InvariantCulture));


            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby published_date
        [HttpGet]
        [Route("published")]

        public async Task<IResult> GetBooksSortByPublished()
        {
            var result = await _genericService.GetBookList();
            result = result.OrderBy(x => x.Publish_Date);

            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby description
        [HttpGet]
        [Route("description")]
        public async Task<IResult> GetBooksSortByDescription()
        {
            var result = await _genericService.GetBookList();
            result = result.OrderBy(x => x.Description);

            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby id and search pattern in id
        [HttpGet]
        [Route("id/{search}")]
        public async Task<IResult> GetBooksSearchId(string search)
        {
            var result = await _genericService.GetBookList();
            result = result.Where(x => x.Id.Contains(search)).OrderBy(x => Convert.ToInt32(x.Id.Remove(0, 1)));

            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.Ok(null);
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby author and search pattern in author
        [HttpGet]
        [Route("author/{search}")]
        public async Task<IResult> GetBooksSearchAuthor(string search)
        {
            var result = await _genericService.GetBookList();
            result = result.Where(x => x.Author.Contains(search, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Author);

            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby title and search pattern in title
        [HttpGet]
        [Route("title/{search}")]
        public async Task<IResult> GetBooksSearchTitle(string search)
        {
            var result = await _genericService.GetBookList();
            result = result.Where(x => x.Title.Contains(search, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Title);

            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby genre and search pattern in genre
        [HttpGet]
        [Route("genre/{search}")]
        public async Task<IResult> GetBooksSearchGenre(string search)
        {
            var result = await _genericService.GetBookList();
            result = result.Where(x => x.Genre.Contains(search, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Genre);

            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby price and search pattern in price
        // Method to get the book orderby price and search pattern in price
        [HttpGet]
        [Route("price/{search}")]
        public async Task<IResult> GetBooksSearchPrice(string search)
        {
            var result = await _genericService.GetBookList();
            result = result.Where(x => x.Price.Contains(search)).OrderBy(x => double.Parse(x.Price, CultureInfo.InvariantCulture));

            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        //// Method to get the book orderby price and search pattern in price
        [HttpGet]
        [Route("price/{minValue:double}&{maxValue:double}")]
        public async Task<IResult> GetBooksSearchPrice(double minValue, double maxValue)
        {
            var result = await _genericService.GetBookList();
            result = result.Where(x => double.Parse(x.Price, CultureInfo.InvariantCulture) >= minValue && double.Parse(x.Price, CultureInfo.InvariantCulture) <= maxValue).OrderBy(x => double.Parse(x.Price, CultureInfo.InvariantCulture));

            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby published and search pattern in published
        [HttpGet]
        [Route("published/{search}")]
        public async Task<IResult> GetBooksSearchPublished(string search)
        {
            var result = await _genericService.GetBookList();
            result = result.Where(x => x.Publish_Date.Contains(search)).OrderBy(x => x.Publish_Date);

            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby published and search pattern in published
        [HttpGet]
        [Route("published/{year:int}")]
        public async Task<IResult> GetBooksSearchPublishedYear(int year)
        {
            string str = year.ToString();

            var result = await _genericService.GetBookList();
            result = result.Where(x => x.Publish_Date.Contains(str)).OrderBy(x => x.Publish_Date);

            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby published and search pattern in published
        [HttpGet]
        [Route("published/{year:int}/{month:int}")]
        public async Task<IResult> GetBooksSearchPublishedMonth(int year, int month)
        {
            string str = year.ToString() + "-" + String.Format("{0:D2}", month);

            var result = await _genericService.GetBookList();
            result = result.Where(x => x.Publish_Date.Contains(str)).OrderBy(x => x.Publish_Date);

            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby published and search pattern in published
        [HttpGet]
        [Route("published/{year:int}/{month:int}/{day:int}")]
        public async Task<IResult> GetBooksSearchPublishedDate(int year, int month, int day)
        {
            string str = year.ToString() + "-" + String.Format("{0:D2}", month) + "-" + String.Format("{0:D2}", day);

            var result = await _genericService.GetBookList();
            result = result.Where(x => x.Publish_Date.Contains(str)).OrderBy(x => x.Publish_Date);

            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to get the book orderby description and search pattern in description
        [HttpGet]
        [Route("description/{search}")]
        public async Task<IResult> GetBooksSearchDescription(string search)
        {
            var result = await _genericService.GetBookList();
            result = result.Where(x => x.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Description);

            //&& x.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
            
            IEnumerable<BookVM> book = _mapper.Map<IEnumerable<BookVM>>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to edit the book detail
        [HttpGet]
        [Route("{id}")]
        public async Task<IResult> EditBookDetail(string id)
        {
            var result = await _genericService.GetBookDetailById(id.ToString());
            BookVM book = _mapper.Map<BookVM>(result);
            if (book == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(book);
        }

        // Method to Save the book detail
        [HttpPost(Name = "SaveBookDetail")]
        public async Task<IResult> SaveBookDetail(BookVM model)
        {
            var result = await _genericService.GetBookList();
            List<int> ids = new List<int>();
            foreach (var item in result)
            {
                ids.Add(Convert.ToInt32(item.Id.Remove(0, 1)));
            }
            int id = ids.Max() + 1;
            model.Id = "B" + id.ToString();
            BookEntity book = _mapper.Map<BookEntity>(model);
            var save = await _genericService.SaveBookDetail(book);
            if (save == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(save);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IResult> UpdateBook([FromRoute] string id, [FromBody] BookVM model)
        {
            BookEntity book = await _genericService.GetBookDetailById(id);
            if (model.Publish_Date != null)
                book.Publish_Date = model.Publish_Date;
            if (model.Title != null)
                book.Title = model.Title;
            if (model.Description != null)
                book.Description = model.Description;
            if (model.Price != null)
                book.Price = model.Price;
            if (model.Genre != null)
                book.Genre = model.Genre;

            var save = await _genericService.UpdateBookDetail(book);
            if (save == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(save);
        }

        // Method to delete the book detail
        [HttpDelete]
        [Route("{id}")]
        public async Task<IResult> DeleteBook(string id)
        {
            await _genericService.DeleteBook(id);
            return Results.Ok();
        }
    }
}
