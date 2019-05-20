﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Shelfy.Core.Domain;
using Shelfy.Core.Helper;
using Shelfy.Core.Repositories;
using Shelfy.Infrastructure.Mongodb;

namespace Shelfy.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository, IMongoRepository
    {
        private readonly IMongoDatabase _database;

        public BookRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<Book> GetByIdAsync(Guid id)
            => await Books.AsQueryable().FirstOrDefaultAsync(x => x.BookId == id);

        public async Task<Book> GetByIsbnAsync(string isbn)
            => await Books.AsQueryable().FirstOrDefaultAsync(x => x.ISBN == isbn);

        public async Task<IEnumerable<Review>> GetBooksReviews()
            => await Books.AsQueryable().SelectMany(x => x.Reviews).ToListAsync();

        public async Task<PagedResult<Book>> BrowseAsync(int currentPage, int pageSize)
            => await Books.AsQueryable().PaginateAsync(currentPage, pageSize);

        public async Task AddAsync(Book book)
            => await Books.InsertOneAsync(book);

        public async Task UpdateAsync(Book book)
            => await Books.ReplaceOneAsync(x => x.BookId == book.BookId, book);

        public async Task RemoveAsync(Guid id)
            => await Books.DeleteOneAsync(x => x.BookId == id);

        private IMongoCollection<Book> Books => _database.GetCollection<Book>("Books");
    }
}