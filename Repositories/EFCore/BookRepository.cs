﻿using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Repositories.EFCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public sealed class BookRepository : RepositoryBase<Book>, IBookRepository
    {
        private RepositoryContext _context;
        public BookRepository(RepositoryContext context) : base(context)
        {
            _context = context;
        }

        public void CreateOneBook(Book book) => Create(book);
        public void DeleteOneBook(Book book) => Delete(book);

        public async Task<PagedList<Book>> GetAllBooksAsync(BookParameters bookParameters, bool trackChanges)
        {
            var books = await FindAll(trackChanges)
                .FilterBooks(bookParameters.MinPrice, bookParameters.MaxPrice)
                .Search(bookParameters.SearchTerm)
                .Sort(bookParameters.OrderBy)
                .ToListAsync();

            //var books = await (trackChanges ? _context.Set<Book>()
            //    .FilterBooks(bookParameters.MinPrice, bookParameters.MaxPrice)
            //    .Search(bookParameters.SearchTerm)
            //    .Sort(bookParameters.OrderBy)
            //    .ToListAsync() :
            //    _context.Set<Book>()
            //    .AsNoTracking()
            //    .FilterBooks(bookParameters.MinPrice, bookParameters.MaxPrice)
            //    .Search(bookParameters.SearchTerm)
            //    .Sort(bookParameters.OrderBy).ToListAsync());

            return PagedList<Book>
                .ToPagedList(books, bookParameters.PageNumber, bookParameters.PageSize);
        }

        public async Task<List<Book>> GetAllBooksAsync(bool trackChanges) =>
            await FindAll(trackChanges)
            .OrderBy(b=>b.Id)
            .ToListAsync();

        public async Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync(bool trackChanges) => 
            await _context
            .Books
            .Include(x => x.Category)
            .OrderBy(x=>x.Id)
            .ToListAsync();
        

        public async Task<Book> GetBookByIdAsync(int id, bool trackChanges) => 
            await FindByCondition(b => b.Id == id, trackChanges)
            .SingleOrDefaultAsync();

        public void UpdateOneBook(Book book) => Update(book);
    }
}
