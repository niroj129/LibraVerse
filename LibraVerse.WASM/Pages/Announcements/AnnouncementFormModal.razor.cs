using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LibraVerse.WASM.Pages.Announcements;

public partial class AnnouncementFormModal
{
    [Parameter] public EventCallback OnSave { get; set; }

    private bool isOpen = false;
    private bool isEditMode = false;
    private bool isSubmitting = false;
    private Guid? editAnnouncementId;
    private string selectedBookId = "";
    private List<BookDto>? books;

    private CreateAnnouncementDto model = new()
    {
        Title = "",
        Description = "",
        Type = AnnouncementType._1,
        StartDate = DateTime.Now,
        EndDate = DateTime.Now.AddDays(7)
    };

    public async Task Open(AnnouncementDto? announcement = null)
    {
        if (announcement != null)
        {
            isEditMode = true;
            editAnnouncementId = announcement.Id;

            model = new CreateAnnouncementDto
            {
                Title = announcement.Title,
                Description = announcement.Description,
                Type = announcement.Type == "General" ? AnnouncementType._1 : AnnouncementType._2,
                StartDate = announcement.StartDate,
                EndDate = announcement.EndDate
            };

            await LoadBooks();

            if (announcement.Book != null)
            {
                selectedBookId = announcement.Book.Id.ToString();
                model.BookId = announcement.Book.Id;
            }
            else
            {
                selectedBookId = "";
                model.BookId = null;
            }
        }
        else
        {
            isEditMode = false;
            editAnnouncementId = null;

            model = new CreateAnnouncementDto
            {
                Title = "",
                Description = "",
                Type = AnnouncementType._1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7)
            };

            selectedBookId = "";
        }

        isOpen = true;
        StateHasChanged();

        if (model.Type == AnnouncementType._2)
        {
            await LoadBooks();
        }
    }

    public void Close()
    {
        isOpen = false;
        StateHasChanged();
    }

    private async Task HandleValidSubmit()
    {
        if (model.Type == AnnouncementType._2 && string.IsNullOrEmpty(selectedBookId))
        {
            Snackbar.Add("Please select a book for offer announcements", Severity.Warning);
            return;
        }

        if (model.EndDate < model.StartDate)
        {
            Snackbar.Add("End date must be after start date", Severity.Warning);
            return;
        }

        isSubmitting = true;
        StateHasChanged();

        try
        {
            if (!string.IsNullOrEmpty(selectedBookId) && Guid.TryParse(selectedBookId, out var bookId))
            {
                model.BookId = bookId;
            }
            else
            {
                model.BookId = null;
            }

            HttpResponseMessage response;

            if (isEditMode && editAnnouncementId.HasValue)
            {
                var updateModel = new UpdateAnnouncementDto
                {
                    Title = model.Title,
                    Description = model.Description,
                    Type = model.Type,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    BookId = model.BookId
                };

                await ApiClient.UpdateAnnouncementAsync(editAnnouncementId ?? Guid.Empty, updateModel);
            }
            else
            {
                await ApiClient.CreateAnnouncementAsync(model);
            }

            Snackbar.Add($"Announcement {(isEditMode ? "updated" : "created")} successfully", Severity.Success);
            Close();
            await OnSave.InvokeAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add("Error: " + ex.Message, Severity.Error);
        }
        finally
        {
            isSubmitting = false;
            StateHasChanged();
        }
    }

    private async Task LoadBooks()
    {
        try
        {
            books = (await ApiClient.GetAllBooksListAsync(null, true, null, null, true)).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load books: " + ex.Message, Severity.Error);
        }
    }

    private async Task HandleTypeChange(ChangeEventArgs e)
    {
        if (e.Value != null && Enum.TryParse<AnnouncementType>(e.Value.ToString(), out var type))
        {
            if (type == AnnouncementType._2 && books == null)
            {
                await LoadBooks();
            }
        }
    }
}