using LibraVerse.Enums;
using LibraVerse.Models;
using LibraVerse.Attribute;
using LibraVerse.Persistence;
using LibraVerse.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using LibraVerse.DTOs.Pagination;
using LibraVerse.Helper.Extension;
using LibraVerse.Controllers.Base;
using LibraVerse.DTOs.Announcement;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraVerse.Controllers;

[Authorize]
public class AnnouncementController(ApplicationDbContext applicationDbContext) : BaseController
{
    [HttpGet]
    [SwaggerOperation(OperationId = "GetAllAnnouncements")]
    public PaginatedResponse<AnnouncementDto> GetAllAnnouncements([FromQuery] PaginationQuery query)
    {
        var announcements = applicationDbContext.Announcements
            .Include(x => x.Book)
            .AsQueryable()
            .ToList();

        var announcementDetails = announcements
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PaginatedResponse<AnnouncementDto>
        {
            Items = announcementDetails.Select(x => x.ToAnnouncementDto(applicationDbContext)).ToList(),
            TotalCount = announcements.Count(),
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
    
    [HttpGet("list")]
    [SwaggerOperation(OperationId = "GetAllAnnouncementsList")]
    public List<AnnouncementDto> GetAllAnnouncements()
    {
        var announcements = applicationDbContext.Announcements
            .Include(x => x.Book)
            .AsQueryable()
            .ToList();

        return announcements.Select(x => x.ToAnnouncementDto(applicationDbContext)).ToList();
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "CreateAnnouncement")]
    public Guid CreateAnnouncement(CreateAnnouncementDto announcement)
    {
        Guid? bookId = null;
        
        if (announcement.Type == AnnouncementType.Offer)
        {
            if (announcement.BookId.HasValue)
            {
                var bookModel = applicationDbContext.Books.Find(announcement.BookId.Value)
                    ?? throw new Exception("Book not found.");
                
                bookId = bookModel.Id;
            }
            else
            {
                throw new Exception("Please select a valid book for offer announcements.");
            }
        }
        
        var announcementModel = new Announcement
        {
            BookId = bookId,
            Title = announcement.Title,
            Description = announcement.Description,
            Type = announcement.Type,
            StartDate = announcement.StartDate.Date,
            EndDate = announcement.EndDate.Date
        };
        
        applicationDbContext.Announcements.Add(announcementModel);
        
        applicationDbContext.SaveChanges();
        
        return announcementModel.Id;
    }
    
    [HttpPut("{announcementId:guid}")]
    [SwaggerOperation(OperationId = "UpdateAnnouncement")]
    public Guid UpdateAnnouncement(Guid announcementId, UpdateAnnouncementDto announcement)
    {
        var announcementModel = applicationDbContext.Announcements.Find(announcementId)
                               ?? throw new Exception("Announcement not found.");
        
        Guid? bookId = null;
        
        if (announcement.Type == AnnouncementType.Offer)
        {
            if (announcement.BookId.HasValue)
            {
                var bookModel = applicationDbContext.Books.Find(announcement.BookId.Value)
                                ?? throw new Exception("Book not found.");
                
                bookId = bookModel.Id;
            }
            else
            {
                throw new Exception("Please select a valid book for offer announcements.");
            }
        }
        
        announcementModel.BookId = bookId;
        announcementModel.Title = announcement.Title;
        announcementModel.Description = announcement.Description;
        announcementModel.Type = announcement.Type;
        announcementModel.StartDate = announcement.StartDate.Date;
        announcementModel.EndDate = announcement.EndDate.Date;
        
        applicationDbContext.Announcements.Update(announcementModel);
        applicationDbContext.SaveChanges();
        
        return announcementModel.Id;
    }
    
    [HttpDelete("{announcementId:guid}")]
    [SwaggerOperation(OperationId = "DeleteAnnouncement")]
    public Guid DeleteAnnouncement(Guid announcementId)
    {
        var announcementModel = applicationDbContext.Announcements.Find(announcementId)
                               ?? throw new Exception("Announcement not found.");
        
        applicationDbContext.Announcements.Remove(announcementModel);
        applicationDbContext.SaveChanges();
        
        return announcementModel.Id;
    }
}