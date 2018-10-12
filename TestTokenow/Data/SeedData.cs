using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTokenow.Data
{
    public class SeedData
    {
        private AppDbContext _context;

        public SeedData(AppDbContext context)
        {
            _context = context;
        }


        public async Task Seed()
        {
            if (!_context.Books.Any())
            {
                var books = new List<Book>();

                for (int i = 0; i < 50; i++)
                {
                    books.Add(new Book { Name = "book " + i });
                }

                _context.Books.AddRange(books);
                await _context.SaveChangesAsync();
            }
        }
    }
}
