using LibraVerse.Models;
using LibraVerse.Persistence;
using LibraVerse.DTOs.Authors;
using Microsoft.AspNetCore.Mvc;
using LibraVerse.DTOs.Response;
using LibraVerse.DTOs.Pagination;
using LibraVerse.Controllers.Base;
using LibraVerse.Helper.Extension;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraVerse.Controllers;

[Authorize]
public class AuthorController(ApplicationDbContext applicationDbContext) : BaseController
{
    [HttpGet]
    [SwaggerOperation(OperationId = "GetAllAuthors")]
    public PaginatedResponse<AuthorDto> GetAllAuthors([FromQuery] PaginationQuery query, string? search = null, bool? isActive = null)
    {
        var authors = applicationDbContext.Authors.AsQueryable();

        var authorDetails = authors
            .Where(x => string.IsNullOrEmpty(search) || x.Name.Contains(search) || x.Email.Contains(search))
            .Where(x => isActive == null || x.IsDeleted == !isActive)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PaginatedResponse<AuthorDto>
        {
            Items = authorDetails.Select(x => x.ToAuthorDto()).ToList(),
            TotalCount = authors.Count(),
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
    
    [HttpGet("list")]
    [SwaggerOperation(OperationId = "GetAllAuthorsList")]
    public List<AuthorDto> GetAllAuthors(string? search = null, bool? isActive = null)
    {
        var authors = applicationDbContext.Authors
            .Where(x => string.IsNullOrEmpty(search) || x.Name.Contains(search) || x.Email.Contains(search))
            .Where(x => isActive == null || x.IsDeleted == !isActive)
            .AsQueryable();
        
        return authors.Select(x => x.ToAuthorDto()).ToList();
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "CreateAuthor")]
    public Guid CreateAuthor(CreateAuthorDto author)
    {
        var authorModel = new Author()
        {
            Name = author.Name,
            Biography = author.Biography,
            Email = author.Email
        };
        
        applicationDbContext.Authors.Add(authorModel);
        
        applicationDbContext.SaveChanges();
        
        return authorModel.Id;
    }
    
    [HttpPut("{authorId:guid}")]
    [SwaggerOperation(OperationId = "UpdateAuthor")]
    public Guid UpdateAuthor(Guid authorId, UpdateAuthorDto author)
    {
        var authorModel = applicationDbContext.Authors.FirstOrDefault(x => x.Id == authorId)
            ?? throw new Exception("Author not found.");
        
        authorModel.Name = author.Name;
        authorModel.Biography = author.Biography;
        authorModel.Email = author.Email;
        
        applicationDbContext.Authors.Update(authorModel);
        applicationDbContext.SaveChanges();
        
        return authorModel.Id;
    }

    [HttpPatch("{authorId:guid}")]
    [SwaggerOperation(OperationId = "UpdateAuthorStatus")]
    public Guid UpdateAuthorStatus(Guid authorId)
    {
        var authorModel = applicationDbContext.Authors.FirstOrDefault(x => x.Id == authorId)
                          ?? throw new Exception("Author not found.");
        
        authorModel.IsDeleted = !authorModel.IsDeleted;

        applicationDbContext.Authors.Update(authorModel);
        applicationDbContext.SaveChanges();
        
        return authorModel.Id;
    }
}