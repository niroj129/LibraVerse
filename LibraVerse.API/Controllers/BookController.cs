using LibraVerse.Attribute;
using LibraVerse.Models;
using LibraVerse.DTOs.Books;
using LibraVerse.Persistence;
using Microsoft.AspNetCore.Mvc;
using LibraVerse.DTOs.Response;
using LibraVerse.DTOs.Pagination;
using LibraVerse.Controllers.Base;
using LibraVerse.Helper.Extension;
using Microsoft.EntityFrameworkCore;
using LibraVerse.Services.Interface;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraVerse.Controllers;

[Authorize]
public class BookController(ApplicationDbContext applicationDbContext, IUserService userService) : BaseController
{
    [HttpGet]
    [SwaggerOperation(OperationId = "GetAllBooks")]
    [ProducesResponseType(typeof(PaginatedResponse<BookDto>), 200)]
    public PaginatedResponse<BookDto> GetAllBooks([FromQuery] PaginationQuery query,
        string? search = null,
        bool? isActive = null,
        Guid? formatId = null,
        Guid? publicationId = null,
        bool? isAvailable = null)
    {
        var userId = userService.UserId;

        var user = applicationDbContext.Users.Find(userId);

        formatId = formatId == Guid.Empty ? null : formatId;
        publicationId = publicationId == Guid.Empty ? null : publicationId;

        var books = applicationDbContext.Books
            .Where(x => isActive == null || x.IsDeleted == !isActive)
            .Where(x => isAvailable == null || x.IsAvailable == isAvailable)
            .Include(x => x.Format)
            .Include(x => x.Publication)
            .Include(x => x.BookAuthors)
            .ThenInclude(x => x.Author)
            .Include(x => x.Reviews)
            .ThenInclude(x => x.User)
            .Where(x => x.BookAuthors.All(z => !x.IsDeleted))
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            var bookInformation = books.Where(x => x.Title.ToLower().Contains(search.ToLower()) || x.Description.Contains(search.ToLower()));

            if (bookInformation.Any())
            {
                books = bookInformation;
            }
        }

        books = search switch
        {
            "Science Fiction" => books.Where(x => x.Genre == Enums.Genre.ScienceFiction),
            "Non Fiction" => books.Where(x => x.Genre == Enums.Genre.NonFiction),
            "Adventure" => books.Where(x => x.Genre == Enums.Genre.Adventure),
            "Biography" => books.Where(x => x.Genre == Enums.Genre.Biography),
            "Thriller" => books.Where(x => x.Genre == Enums.Genre.Thriller),
            "Children" => books.Where(x => x.Genre == Enums.Genre.Children),
            "Romance" => books.Where(x => x.Genre == Enums.Genre.Romance),
            "Fiction" => books.Where(x => x.Genre == Enums.Genre.Fiction),
            "Fantasy" => books.Where(x => x.Genre == Enums.Genre.Fantasy),
            _ => books
        };

        if (formatId.HasValue)
        {
            books = books.Where(x => x.FormatId == formatId.Value);
        }

        if (publicationId.HasValue)
        {
            books = books.Where(x => x.PublicationId == publicationId.Value);
        }

        var bookDetails = books
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PaginatedResponse<BookDto>
        {
            Items = bookDetails.Select(x => x.ToBookDto(applicationDbContext, user?.Id)).ToList(),
            TotalCount = books.Count(),
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
    
    [HttpGet("list")]
    [SwaggerOperation(OperationId = "GetAllBooksList")]
    public List<BookDto> GetAllBooks(string? search = null,
        bool? isActive = null,
        Guid? formatId = null,
        Guid? publicationId = null,
        bool? isAvailable = null)
    {
        var userId = userService.UserId;

        var user = applicationDbContext.Users.Find(userId);

        var userIdentifier = user?.Id;
        
        var books = applicationDbContext.Books
            .Where(x => isActive == null || x.IsDeleted == !isActive)
            .Where(x => isAvailable == null || x.IsAvailable == isAvailable)
            .Include(x => x.Format)
            .Include(x => x.Publication)
            .Include(x => x.Discounts)
            .Include(x => x.BookAuthors)
            .ThenInclude(x => x.Author)
            .Include(x => x.Reviews)
            .ThenInclude(x => x.User)
            .Where(x => x.BookAuthors.All(z => !x.IsDeleted))
            .AsQueryable()
            .ToList();
        
        if (!string.IsNullOrEmpty(search))
        {
            books = books.Where(x => x.Title.Contains(search) || x.Description.Contains(search)).ToList();
        }

        if (formatId.HasValue)
        {
            books = books.Where(x => x.FormatId == formatId.Value).ToList();
        }

        if (publicationId.HasValue)
        {
            books = books.Where(x => x.PublicationId == publicationId.Value).ToList();
        }
        
        return books.ToList().Select(x => x.ToBookDto(applicationDbContext, userIdentifier)).ToList();
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "CreateBookDto")]
    public Guid CreateBook([FromForm] CreateBookDto book)
    {
        var format = applicationDbContext.Formats.Find(book.FormatId)
            ?? throw new Exception("Format not found.");
        
        var publication = applicationDbContext.Publications.Find(book.PublicationId)
            ?? throw new Exception("Publication not found.");
        
        var authors = applicationDbContext.Authors.Where(x => book.AuthorIds.Contains(x.Id)).ToList();

        var coverImage = book.CoverImage != null ? UploadImage(book.CoverImage) : string.Empty;
        
        var authorModel = new Book()
        {
            FormatId = format.Id,
            PublicationId = publication.Id,
            Title = book.Title,
            Description = book.Description,
            CoverImage = coverImage,
            Iban = book.Iban,
            IsAvailable = book.IsAvailable,
            Stock = book.Stock,
            Language = book.Language,
            Genre = book.Genre,
            Price = book.Price,
            PublishedDate = book.PublishedDate,
            BookAuthors = authors.Select(x => new BookAuthors()
            {
                AuthorId = x.Id
            }).ToList()
        };
        
        applicationDbContext.Books.Add(authorModel);
        
        applicationDbContext.SaveChanges();
        
        return authorModel.Id;
    }
    
    [HttpPut("{bookId:guid}")]
    [SwaggerOperation(OperationId = "UpdateBook")]
    public Guid UpdateBook(Guid bookId, [FromForm] UpdateBookDto book)
    {
        var format = applicationDbContext.Formats.Find(book.FormatId)
                     ?? throw new Exception("Format not found.");
        
        var publication = applicationDbContext.Publications.Find(book.PublicationId)
                          ?? throw new Exception("Publication not found.");
        
        var authors = applicationDbContext.Authors.Where(x => book.AuthorIds.Contains(x.Id)).ToList();
        
        var bookModel = applicationDbContext.Books.Find(bookId)
            ?? throw new Exception("Book not found.");
        
        var bookAuthors = applicationDbContext.BookAuthors.Where(x => x.BookId == bookModel.Id).ToList();
        
        applicationDbContext.BookAuthors.RemoveRange(bookAuthors);
        applicationDbContext.SaveChanges();

        bookModel.FormatId = format.Id;
        bookModel.PublicationId = publication.Id;
        bookModel.Iban = book.Iban;
        bookModel.IsAvailable = book.IsAvailable;
        bookModel.Stock = book.Stock;
        bookModel.Language = book.Language;
        bookModel.Genre = book.Genre;
        bookModel.Price = book.Price;
        bookModel.PublishedDate = book.PublishedDate;
        bookModel.Title = book.Title;
        bookModel.Description = book.Description;
        bookModel.BookAuthors = authors.Select(x => new BookAuthors()
        {
            AuthorId = x.Id
        }).ToList();

        if (book.CoverImage != null)
        {
            DeleteImage(bookModel.CoverImage);
            
            var coverImage = book.CoverImage != null ? UploadImage(book.CoverImage) : string.Empty;

            bookModel.CoverImage = coverImage;
        }
        
        applicationDbContext.Books.Update(bookModel);
        applicationDbContext.SaveChanges();
        
        return bookModel.Id;
    }

    [HttpPatch("{bookId:guid}")]
    [SwaggerOperation(OperationId = "UpdateBookStatus")]
    public Guid UpdateBookStatus(Guid bookId)
    {
        var bookModel = applicationDbContext.Books.Find(bookId)
                          ?? throw new Exception("Book not found.");
        
        bookModel.IsDeleted = !bookModel.IsDeleted;

        applicationDbContext.Books.Update(bookModel);
        applicationDbContext.SaveChanges();
        
        return bookModel.Id;
    }
    
    [HttpPatch("{authorId:guid}/availability")]
    [SwaggerOperation(OperationId = "UpdateBookAvailabilityStatus")]
    public Guid UpdateBookAvailabilityStatus(Guid authorId)
    {
        var bookModel = applicationDbContext.Books.Find(authorId)
                        ?? throw new Exception("Book not found.");
        
        bookModel.IsAvailable = !bookModel.IsAvailable;

        applicationDbContext.Books.Update(bookModel);
        applicationDbContext.SaveChanges();
        
        return bookModel.Id;
    }
    
    [NonAction]
    private static string UploadImage(IFormFile file)
    {
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        
        file.CopyTo(stream);

        return fileName;
    }

    [NonAction]
    private static void DeleteImage(string fileName)
    {
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        var filePath = Path.Combine(folderPath, fileName);

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
        else
        {
            Console.WriteLine($"File not found: {filePath}.");
        }
    }

}