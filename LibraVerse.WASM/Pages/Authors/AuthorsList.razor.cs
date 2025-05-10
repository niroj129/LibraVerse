using MudBlazor;
using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components;

namespace LibraVerse.WASM.Pages.Authors;

public partial class AuthorsList
{
    private AuthorDtoPaginatedResponse? _authors;
    private bool _isLoading = true;
    private string _searchTerm = "";
    private string? _activeFilter = "";
    private int _currentPage = 1;
    private readonly int _pageSize = 8;
    private string _viewMode = "grid";
    private AuthorFormModal AuthorFormModal;
    private CreateAuthorDto? authorFormModal;
    private System.Timers.Timer? _searchTimer;
    private bool? _active;
    
    protected override async Task OnInitializedAsync()
    {
        _searchTimer = new System.Timers.Timer(500);
        _searchTimer.Elapsed += async (sender, e) => await SearchAuthors();
        _searchTimer.AutoReset = false;

        await LoadAuthors();
    }

    private async Task HandleStatusChange(ChangeEventArgs e)
    {
        if (e.Value != null)
        {
            _activeFilter = e.Value.ToString();
            _active = _activeFilter == "Active" ? true : _activeFilter == "Inactive" ? false : null;
        }

        await LoadAuthors();
    }
    
    private async Task LoadAuthors()
    {
        _isLoading = true;
        StateHasChanged();

        try
        {
            _authors = await ApiClient.GetAllAuthorsAsync(_currentPage, _pageSize, _searchTerm, _active);
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load authors: " + ex.Message, Severity.Error);
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private void HandleSearch()
    {
        _searchTimer?.Stop();
        _searchTimer?.Start();
    }

    private async Task SearchAuthors()
    {
        await InvokeAsync(async () =>
        {
            _currentPage = 1;
            await LoadAuthors();
        });
    }

    private async Task ChangePage(int page)
    {
        _currentPage = page;
        await LoadAuthors();
    }

    private void OpenCreateAuthorDialog()
    {
        AuthorFormModal.Open(null);
    }

    private void OpenEditAuthorDialog(AuthorDto author)
    {
        AuthorFormModal.Open(author);
    }

    private async Task ConfirmDeleteAuthor(AuthorDto author)
    {
        var message = author.IsActive ? "Deactivate" : "Activate";
        
        var result = await DialogService.ShowMessageBox(
            "Activation Status Confirmation",
            $"Are you sure you want to {message.ToLower()} '{author.Name}'?",
            message,
            "Cancel"
        );

        if (result == true)
        {
            await DeleteAuthor(author.Id);
        }
    }

    private async Task DeleteAuthor(Guid id)
    {
        try
        {
            await ApiClient.UpdateAuthorStatusAsync(id);

            await LoadAuthors();
            
            Snackbar.Add("Author status updated successfully", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    public void Dispose()
    {
        _searchTimer?.Dispose();
    }

    private void SetViewMode(string view)
    {
        _viewMode = view;
    }
}