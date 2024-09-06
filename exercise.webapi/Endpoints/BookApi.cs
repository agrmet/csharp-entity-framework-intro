﻿using exercise.webapi.DTOs;
using exercise.webapi.Models;
using exercise.webapi.Repository;

namespace exercise.webapi.Endpoints
{
    public static class BookApi
    {
        public static void ConfigureBooksApi(this WebApplication app)
        {
            var books = app.MapGroup("books");
            books.MapGet("/", GetBooks);
            books.MapGet("/{id}", GetABook);
            books.MapPut("/{id}/{authorid}", UpdateAuthor);
            books.MapDelete("/{id}", DeleteBook);
        }


        private static async Task<IResult> GetBooks(IBookRepository bookRepository)
        {
            var books = await bookRepository.GetAll();
            List<BookDTO> result = [];
            foreach (var book in books)
            {
                var bookdto = MapToBookDTO(book);
                result.Add(bookdto);
            }
            return TypedResults.Ok(result);
        }
        private static async Task<IResult> GetABook(IBookRepository bookRepository, int id)
        {
            var book = await bookRepository.GetA(id);
            if (book == null) return TypedResults.NotFound("Book was not found");
            var bookdto = MapToBookDTO(book);
            return TypedResults.Ok(bookdto);
        }


        private static async Task<IResult> UpdateAuthor(IBookRepository bookRepository, IAuthorRepository authorRepository, int bookid, int authorid) 
        {
            var book = await bookRepository.GetA(bookid);
            if (book is null) return TypedResults.NotFound("Book was not found");
            var author = await authorRepository.GetAuthor(authorid);
            if (author is null) return TypedResults.NotFound("Author was not found");
            book.Author = author;
            book.AuthorId = authorid;
            bookRepository.Update(book);
            return TypedResults.Ok(MapToBookDTO(book));
        }

        private static async Task<IResult> DeleteBook(IBookRepository bookRepository, int id)
        {
            var book = await bookRepository.Delete(id);
            return TypedResults.Ok(MapToBookDTO(book));
        }
        private static BookDTO MapToBookDTO(Book book)
        {
            return new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                AuthorId = book.AuthorId,
                Author = $"{book.Author.FirstName} {book.Author.LastName}"
            };
        }

        private static AuthorDTO MapToAuthorDTO(Author author)
        {
            return new AuthorDTO
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                Email = author.Email
            };
        }
    }
}
