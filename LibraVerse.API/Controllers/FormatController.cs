using LibraVerse.Models;
using LibraVerse.Attribute;
using LibraVerse.Persistence;
using LibraVerse.DTOs.Formats;
using Microsoft.AspNetCore.Mvc;
using LibraVerse.DTOs.Response;
using LibraVerse.DTOs.Pagination;
using LibraVerse.Controllers.Base;
using LibraVerse.Helper.Extension;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraVerse.Controllers;

[Authorize]
public class FormatController(ApplicationDbContext applicationDbContext) : BaseController
{
    [HttpGet]
    [SwaggerOperation(OperationId = "GetAllFormats")]
    public PaginatedResponse<FormatDto> GetAllFormats([FromQuery] PaginationQuery query, string? search = null, bool? isActive = null)
    {
        var formats = applicationDbContext.Formats.AsQueryable();

        var formatDetails = formats
            .Where(x => string.IsNullOrEmpty(search) || x.Title.Contains(search) || x.Description.Contains(search))
            .Where(x => isActive == null || x.IsDeleted == !isActive)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PaginatedResponse<FormatDto>
        {
            Items = formatDetails.Select(x => x.ToFormatDto()).ToList(),
            TotalCount = formats.Count(),
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
    
    [HttpGet("list")]
    [SwaggerOperation(OperationId = "GetAllFormatsList")]
    public List<FormatDto> GetAllFormats(string? search = null, bool? isActive = null)
    {
        var formats = applicationDbContext.Formats
            .Where(x => string.IsNullOrEmpty(search) || x.Title.Contains(search) || x.Description.Contains(search))
            .Where(x => isActive == null || x.IsDeleted == !isActive)
            .AsQueryable();
        
        return formats.Select(x => x.ToFormatDto()).ToList();
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "CreateFormat")]
    public Guid CreateFormat(CreateFormatDto format)
    {
        var formatModel = new Format()
        {
            Title = format.Title,
            Description = format.Description
        };
        
        applicationDbContext.Formats.Add(formatModel);
        
        applicationDbContext.SaveChanges();
        
        return formatModel.Id;
    }
    
    [HttpPut("{authorId:guid}")]
    [SwaggerOperation(OperationId = "UpdateFormat")]
    public Guid UpdateFormat(Guid authorId, UpdateFormatDto format)
    {
        var formatModel = applicationDbContext.Formats.Find(authorId)
            ?? throw new Exception("Format not found.");
        
        formatModel.Title = format.Title;
        formatModel.Description = format.Description;
        
        applicationDbContext.Formats.Update(formatModel);
        applicationDbContext.SaveChanges();
        
        return formatModel.Id;
    }

    [HttpPatch("{authorId:guid}")]
    [SwaggerOperation(OperationId = "UpdateFormatStatus")]
    public Guid UpdateFormatStatus(Guid authorId)
    {
        var formatModel = applicationDbContext.Formats.Find(authorId)
                          ?? throw new Exception("Format not found.");
        
        formatModel.IsDeleted = !formatModel.IsDeleted;

        applicationDbContext.Formats.Update(formatModel);
        applicationDbContext.SaveChanges();
        
        return formatModel.Id;
    }
}