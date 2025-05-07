using LibraVerse.Models;
using LibraVerse.Attribute;
using LibraVerse.Persistence;
using LibraVerse.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using LibraVerse.DTOs.Pagination;
using LibraVerse.Helper.Extension;
using LibraVerse.Controllers.Base;
using LibraVerse.DTOs.Publications;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraVerse.Controllers;

[Authorize]
public class PublicationController(ApplicationDbContext applicationDbContext) : BaseController
{
    [HttpGet] 
    [SwaggerOperation(OperationId = "GetAllPublications")]
    public PaginatedResponse<PublicationDto> GetAllPublications([FromQuery] PaginationQuery query, string? search = null, bool? isActive = null)
    {
        var publications = applicationDbContext.Publications.AsQueryable();

        var publicationDetails = publications
            .Where(x => string.IsNullOrEmpty(search) || x.Title.Contains(search) || x.Description.Contains(search))
            .Where(x => isActive == null || x.IsDeleted == !isActive)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PaginatedResponse<PublicationDto>
        {
            Items = publicationDetails.Select(x => x.ToPublicationDto()).ToList(),
            TotalCount = publications.Count(),
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
    
    [HttpGet("list")]
    [SwaggerOperation(OperationId = "GetAllPublicationsList")]
    public List<PublicationDto> GetAllPublications(string? search = null, bool? isActive = null)
    {
        var publications = applicationDbContext.Publications
            .Where(x => string.IsNullOrEmpty(search) || x.Title.Contains(search) || x.Description.Contains(search))
            .Where(x => isActive == null || x.IsDeleted == !isActive)
            .AsQueryable();
        
        return publications.Select(x => x.ToPublicationDto()).ToList();
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "CreatePublication")]
    public Guid CreatePublication(CreatePublicationDto publication)
    {
        var publicationModel = new Publication()
        {
            Title = publication.Title,
            Description = publication.Description
        };
        
        applicationDbContext.Publications.Add(publicationModel);
        
        applicationDbContext.SaveChanges();
        
        return publicationModel.Id;
    }
    
    [HttpPut("{publicationId:guid}")]
    [SwaggerOperation(OperationId = "UpdatePublication")]
    public Guid UpdatePublication(Guid publicationId, UpdatePublicationDto publication)
    {
        var publicationModel = applicationDbContext.Publications.Find(publicationId)
            ?? throw new Exception("Publication not found.");
        
        publicationModel.Title = publication.Title;
        publicationModel.Description = publication.Description;
        
        applicationDbContext.Publications.Update(publicationModel);
        applicationDbContext.SaveChanges();
        
        return publicationModel.Id;
    }

    [HttpPatch("{publicationId:guid}")]
    [SwaggerOperation(OperationId = "UpdatePublicationStatus")]
    public Guid UpdatePublicationStatus(Guid publicationId)
    {
        var publicationModel = applicationDbContext.Publications.Find(publicationId)
                          ?? throw new Exception("Publication not found.");
        
        publicationModel.IsDeleted = !publicationModel.IsDeleted;

        applicationDbContext.Publications.Update(publicationModel);
        applicationDbContext.SaveChanges();
        
        return publicationModel.Id;
    }
}