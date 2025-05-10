using MudBlazor;
using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components;

namespace LibraVerse.WASM.Pages.Publications;

public partial class PublicationsList
{
    private PublicationDtoPaginatedResponse? _publications;
    private bool _isLoading = true;
    private string _searchTerm = "";
    private string? _activeFilter = "";
    private int _currentPage = 1;
    private readonly int _pageSize = 8;
    private string _viewMode = "grid";
    private PublicationFormModal PublicationFormModal;
    private CreatePublicationDto? publicationFormModal;
    private System.Timers.Timer? _searchTimer;
    private bool? _active;
    
    protected override async Task OnInitializedAsync()
    {
        _searchTimer = new System.Timers.Timer(500);
        _searchTimer.Elapsed += async (sender, e) => await SearchPublications();
        _searchTimer.AutoReset = false;

        await LoadPublications();
    }

    private async Task HandleStatusChange(ChangeEventArgs e)
    {
        if (e.Value != null)
        {
            _activeFilter = e.Value.ToString();
            _active = _activeFilter == "Active" ? true : _activeFilter == "Inactive" ? false : null;
        }

        await LoadPublications();
    }
    
    private async Task LoadPublications()
    {
        _isLoading = true;
        StateHasChanged();

        try
        {
            _publications = await ApiClient.GetAllPublicationsAsync(_currentPage, _pageSize, _searchTerm, _active);
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load publications: " + ex.Message, Severity.Error);
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

    private async Task SearchPublications()
    {
        await InvokeAsync(async () =>
        {
            _currentPage = 1;
            await LoadPublications();
        });
    }

    private async Task ChangePage(int page)
    {
        _currentPage = page;
        await LoadPublications();
    }

    private void OpenCreatePublicationDialog()
    {
        PublicationFormModal.Open(null);
    }

    private void OpenEditPublicationDialog(PublicationDto publication)
    {
        PublicationFormModal.Open(publication);
    }

    private async Task ConfirmDeletePublication(PublicationDto publication)
    {
        var message = publication.IsActive ? "Deactivate" : "Activate";
        
        var result = await DialogService.ShowMessageBox(
            "Activation Status Confirmation",
            $"Are you sure you want to {message.ToLower()} '{publication.Title}'?",
            message,
            "Cancel"
        );

        if (result == true)
        {
            await DeletePublication(publication.Id);
        }
    }

    private async Task DeletePublication(Guid id)
    {
        try
        {
            await ApiClient.UpdatePublicationStatusAsync(id);

            await LoadPublications();
            
            Snackbar.Add("Publication status updated successfully", Severity.Success);
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