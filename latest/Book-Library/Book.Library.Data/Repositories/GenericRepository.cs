using Book.Library.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Book.Library.Data.Repositories
{
    public interface IGenericRepository
    {
        Task<IEnumerable<BookEntity>> GetBookList();
        Task<BookEntity> GetBookDetailById(string bookId);
        Task<BookEntity> SaveBookDetail(BookEntity model);
        Task<BookEntity> UpdateBookDetail(BookEntity model);
        Task DeleteBook(string bookId);
    }

    public class GenericRepository : IGenericRepository
    {
        private readonly ApplicationDbContext _model;
        public GenericRepository(ApplicationDbContext model)
        {
            _model = model;
        }

        public async Task<IEnumerable<BookEntity>> GetBookList()
        {
            var model = await _model.Books.ToListAsync();
            return model;
        }

        public async Task<BookEntity> GetBookDetailById(string bookId)
        {
            return await _model.Books.FindAsync(bookId);
        }

        public async Task<BookEntity> SaveBookDetail(BookEntity model)
        {
            await _model.Books.AddAsync(model);
            await _model.SaveChangesAsync();
            return model;
        }

        public async Task<BookEntity> UpdateBookDetail(BookEntity model)
        {
            _model.Update(model);
            await _model.SaveChangesAsync();
            return model;
        }

        public async Task DeleteBook(string bookId)
        {
            BookEntity book = await _model.Books.FindAsync(bookId);
            if (book != null)
            {
                _model.Remove(book);
                await _model.SaveChangesAsync();
            }
        }
    }
}
