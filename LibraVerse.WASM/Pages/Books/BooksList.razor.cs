using System.Net.Http.Json;
using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LibraVerse.WASM.Pages.Books;

public partial class BooksList
{
    private BookDtoPaginatedResponse? _books;
    private List<FormatDto> _formats = new();
    private List<PublicationDto> _publications = new();
    private bool _isLoading = true;
    private string _searchTerm = "";
    private string _availabilityFilter = "";
    private string _selectedFormat = "";
    private string _selectedPublication = "";
    private int _currentPage = 1;
    private int _pageSize = 12;
    private string _viewMode = "grid";
    private BookFormModal? bookFormModal;
    private System.Timers.Timer? searchTimer;

    protected override async Task OnInitializedAsync()
    {
        searchTimer = new System.Timers.Timer(500);
        searchTimer.Elapsed += async (sender, e) => await SearchBooks();
        searchTimer.AutoReset = false;

        await LoadFormats();
        await LoadPublications();
        await LoadBooks();
    }

    private async Task LoadFormats()
    {
        try
        {
            _formats = (await ApiClient.GetAllFormatsListAsync(null, true)).ToList();
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
            _publications = (await ApiClient.GetAllPublicationsListAsync(null, true)).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load publications: " + ex.Message, Severity.Error);
        }
    }

    private async Task HandleFormatChange(ChangeEventArgs e)
    {
        if (e.Value != null)
        {
            _selectedFormat = e.Value.ToString() ?? "";
        }

        await LoadBooks();
    }
    
    private async Task HandlePublicationChange(ChangeEventArgs e)
    {
        if (e.Value != null)
        {
            _selectedPublication = e.Value.ToString() ?? "";
        }

        await LoadBooks();
    }
    
    private async Task HandleStatusChange(ChangeEventArgs e)
    {
        if (e.Value != null)
        {
            var filter = e.Value.ToString();
            _availabilityFilter = filter ?? string.Empty;
        }

        await LoadBooks();
    }
    
    private async Task LoadBooks()
    {
        _isLoading = true;
        StateHasChanged();

        try
        {
            Guid? _selectedFormatId = string.IsNullOrEmpty(_selectedFormat) ? null : Guid.Parse(_selectedFormat);
            Guid? _selectedPublicationId = string.IsNullOrEmpty(_selectedPublication) ? null : Guid.Parse(_selectedPublication);
            bool? _availability = _availabilityFilter switch
            {
                "Available" => true,
                "Unavailable" => false,
                _ => null
            };
            var books = await ApiClient.GetAllBooksAsync(_currentPage, _pageSize, _searchTerm, true, _selectedFormatId, _selectedPublicationId, _availability);
            _books = books;
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private void HandleSearch()
    {
        searchTimer?.Stop();
        searchTimer?.Start();
    }

    private async Task SearchBooks()
    {
        await InvokeAsync(async () =>
        {
            _currentPage = 1;
            await LoadBooks();
        });
    }

    private async Task ApplyFilters()
    {
        _currentPage = 1;
        await LoadBooks();
    }

    private async Task ChangePage(int page)
    {
        _currentPage = page;
        await LoadBooks();
    }

    private void OpenCreateBookDialog()
    {
        bookFormModal?.Open(null);
    }

    private void OpenEditBookDialog(BookDto book)
    {
        bookFormModal?.Open(book);
    }

    private async Task ToggleAvailability(Guid bookId)
    {
        try
        {
            await ApiClient.UpdateBookAvailabilityStatusAsync(bookId);
            
            Snackbar.Add("Book availability updated successfully", Severity.Success);

            await LoadBooks();
        }
        catch (Exception ex)
        {
            Snackbar.Add("Error: " + ex.Message, Severity.Error);
        }
    }

    private void NavigateToDetails(Guid bookId)
    {
        NavigationManager.NavigateTo($"/books/{bookId}/details");
    }

    public void Dispose()
    {
        searchTimer?.Dispose();
    }

    private void SetViewMode(string view)
    {
        _viewMode = view;
    }
}