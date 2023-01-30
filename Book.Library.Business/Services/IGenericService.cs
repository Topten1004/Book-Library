using Book.Library.Data.Entities;
using Book.Library.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book.Library.Business.Services
{
    public interface IGenericService
    {
        Task<IEnumerable<BookEntity>> GetBookList();

        Task<BookEntity> GetBookDetailById(string bookId);

        Task<BookEntity> SaveBookDetail(BookEntity model);

        Task<BookEntity> UpdateBookDetail(BookEntity model);

        Task DeleteBook(string bookId);
    }

    public class GenericService : IGenericService
    {
        private readonly IGenericRepository _userRepository;

        public GenericService(IGenericRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<BookEntity>> GetBookList()
        {
            return await _userRepository.GetBookList();
        }

        public async Task<BookEntity> GetBookDetailById(string bookId)
        {
            return await _userRepository.GetBookDetailById(bookId);
        }

        public async Task<BookEntity> SaveBookDetail(BookEntity model)
        {
            return await _userRepository.SaveBookDetail(model);
        }

        public async Task<BookEntity> UpdateBookDetail(BookEntity model)
        {
            return await _userRepository.UpdateBookDetail(model);
        }

        public async Task DeleteBook(string bookId)
        {
            await _userRepository.DeleteBook(bookId);
        }
    }
}
