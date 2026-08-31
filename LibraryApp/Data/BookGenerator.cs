using LibraryApp.Models;
using Bogus;
using System;
using System.Collections.Generic;

namespace LibraryApp.Data
{
    public class BookGenerator
    {
        public static IEnumerable<Book> Generate(int count)
        {
            var faker = new Faker();
            var books = new List<Book>();
            for (int i = 0; i < count; i++)
            {
                var book = new Book
                {
                    Title = faker.Commerce.ProductName(),
                    Author = faker.Name.FullName(),
                    Description = faker.Commerce.ProductDescription(),
                    Publisher = faker.Company.CompanyName(),
                    Category = faker.Commerce.Department(),
                    ISBN = faker.Commerce.Ean13(),
                    PageCount = faker.Random.Int(100, 500),
                    PublicationDate = faker.Date.Past(50),
                    CoverImage = "https://example.com/covers/" + faker.Commerce.ProductName().Replace(" ", "_") + ".jpg",
                    IsAvailable = faker.Random.Bool()
                };
                
                books.Add(book);
            }
            return books;
        }
    }
}
