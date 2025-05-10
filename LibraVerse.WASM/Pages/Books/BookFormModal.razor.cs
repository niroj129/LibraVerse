using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace LibraVerse.WASM.Pages.Books;

public partial class BookFormModal
{
    [Parameter] public EventCallback OnSave { get; set; }

    private bool isOpen = false;
    private bool isEditMode = false;
    private bool isSaving = false;
    private Guid? bookId;

    private BookDto bookModel = new()
    {
        Authors = [],
        Publication = new(),
        Format = new(),
        Discount = new(),
        Discounts = [],
        Reviews = [],
        Genre = "Science Fiction",
        Language = "Russian"
    };
    private List<FormatDto> formats = new();
    private List<PublicationDto> publications = new();
    private List<AuthorDto> authors = new();
    private Guid _formatId = Guid.Empty;
    private Guid _publicationId = Guid.Empty;
    private List<Guid> selectedAuthors = new();
    private IBrowserFile? selectedFile;
    private string? coverImageUrl;

    protected override async Task OnInitializedAsync()
    {
        await LoadFormats();
        await LoadPublications();
        await LoadAuthors();
    }

    private async Task LoadFormats()
    {
        try
        {
            var result = await ApiClient.GetAllFormatsListAsync(null, true);
            formats = result.ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load formats: " + ex.Message, Severity.Error);
        }
    }

    private async Task LoadPublications()
    {
        try
        {
            var result = await ApiClient.GetAllPublicationsListAsync(null, true);

            publications = result.ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load publications: " + ex.Message, Severity.Error);
        }
    }

    private async Task LoadAuthors()
    {
        try
        {
            var result = await ApiClient.GetAllAuthorsListAsync(null, true);

            authors = result.ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load authors: " + ex.Message, Severity.Error);
        }
    }

    public void Open(BookDto? book)
    {
        if (book != null)
        {
            isEditMode = true;
            bookId = book.Id;
            bookModel = book;

            _formatId = bookModel.Format.Id;
            _publicationId = bookModel.Publication.Id;
            
            selectedAuthors = book.Authors.Select(a => a.Id).ToList();

            coverImageUrl = !string.IsNullOrEmpty(book.CoverImage) ? $"https://localhost:7115/images/{book.CoverImage}" : null;
        }
        else
        {
            isEditMode = false;
            bookId = null;
            bookModel = new BookDto
            {
                PublishedDate = DateTime.Now,
                IsAvailable = true
            };
            selectedAuthors = new List<Guid>();
            coverImageUrl = null;
            selectedFile = null;
        }

        isOpen = true;
        StateHasChanged();
    }

    public void Close()
    {
        isOpen = false;
        StateHasChanged();
    }

    private void ToggleAuthor(Guid authorId, bool isChecked)
    {
        if (isChecked)
        {
            if (!selectedAuthors.Contains(authorId))
            {
                selectedAuthors.Add(authorId);
            }
        }
        else
        {
            selectedAuthors.Remove(authorId);
        }
    }

    private async Task OnFileChange(InputFileChangeEventArgs e)
    {
        selectedFile = e.File;

        try
        {
            var resizedImage = await e.File.RequestImageFileAsync("image/jpeg", 800, 1200);
            var buffer = new byte[resizedImage.Size];
            await resizedImage.OpenReadStream().ReadAsync(buffer);
            coverImageUrl = $"data:image/jpeg;base64,{Convert.ToBase64String(buffer)}";
        }
        catch (Exception ex)
        {
            Snackbar.Add("Error processing image: " + ex.Message, Severity.Error);
        }
    }

    private async Task HandleSubmit()
    {
        if (!selectedAuthors.Any())
        {
            return;
        }

        isSaving = true;
        StateHasChanged();

        try
        {
            Genre genre = bookModel.Genre switch
            {
                "Science Fiction" => Genre._1,
                "Non Fiction"     => Genre._2,
                "Adventure"       => Genre._3,
                "Biography"       => Genre._4,
                "Thriller"        => Genre._5,
                "Children"        => Genre._6,
                "Romance"         => Genre._7,
                "Fiction"         => Genre._8,
                "Fantasy"         => Genre._9,
                _                 => throw new ArgumentException($"Unknown genre: {bookModel.Genre}")
            };
            
            Language language = bookModel.Language switch
            {
                "Russian" => Language._1,
                "English" => Language._2,
                "Spanish"  => Language._3,
                "French"   => Language._4,
                "German"   => Language._5,
                "Nepali"  => Language._6,
                _          => throw new ArgumentException($"Unknown language: {bookModel.Language}")
            };
            
            var fileParam = await ConvertToFileParameterAsync(selectedFile);
            
            if (isEditMode && bookId.HasValue)
            {
                await ApiClient.UpdateBookAsync(bookId ?? Guid.Empty,
                    _formatId,
                    _publicationId,
                    selectedAuthors,
                    bookModel.Title,
                    bookModel.Description,
                    bookModel.Iban,
                    language,
                    genre,
                    bookModel.Price,
                    bookModel.IsAvailable,
                    bookModel.Stock,
                    bookModel.PublishedDate,
                    fileParam);
            }
            else
            {
                await ApiClient.CreateBookDtoAsync(
                    _formatId,
                    _publicationId,
                    selectedAuthors,
                    bookModel.Title,
                    bookModel.Description,
                    bookModel.Iban,
                    language,
                    genre,
                    bookModel.Price,
                    bookModel.IsAvailable,
                    bookModel.Stock,
                    bookModel.PublishedDate.Date,
                    fileParam);
            }

            Snackbar.Add($"Book {(isEditMode ? "updated" : "created")} successfully", Severity.Success);
            
            Close();

            await OnSave.InvokeAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            isSaving = false;
        }
    }
    
    private async Task<FileParameter?> ConvertToFileParameterAsync(IBrowserFile? browserFile)
    {
        if (browserFile == null) return null;

        var stream = browserFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10MB max, change as needed
        var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        memoryStream.Position = 0; // Reset to start

        return new FileParameter(memoryStream, browserFile.Name, browserFile.ContentType);
    }
}