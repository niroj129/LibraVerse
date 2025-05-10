using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LibraVerse.WASM.Pages.Formats;

public partial class FormatsList
{
    private FormatDtoPaginatedResponse? _formats;
    private bool _isLoading = true;
    private string _searchTerm = "";
    private string? _activeFilter = "";
    private int _currentPage = 1;
    private readonly int _pageSize = 8;
    private string _viewMode = "grid";
    private FormatFormModal FormatFormModal;
    private CreateFormatDto? formatFormModal;
    private System.Timers.Timer? _searchTimer;
    private bool? _active;
    
    protected override async Task OnInitializedAsync()
    {
        _searchTimer = new System.Timers.Timer(500);
        _searchTimer.Elapsed += async (sender, e) => await SearchFormats();
        _searchTimer.AutoReset = false;

        await LoadFormats();
    }

    private async Task HandleStatusChange(ChangeEventArgs e)
    {
        if (e.Value != null)
        {
            _activeFilter = e.Value.ToString();
            _active = _activeFilter == "Active" ? true : _activeFilter == "Inactive" ? false : null;
        }

        await LoadFormats();
    }
    
    private async Task LoadFormats()
    {
        _isLoading = true;
        StateHasChanged();

        try
        {
            _formats = await ApiClient.GetAllFormatsAsync(_currentPage, _pageSize, _searchTerm, _active);
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load formats: " + ex.Message, Severity.Error);
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

    private async Task SearchFormats()
    {
        await InvokeAsync(async () =>
        {
            _currentPage = 1;
            await LoadFormats();
        });
    }

    private async Task ChangePage(int page)
    {
        _currentPage = page;
        await LoadFormats();
    }

    private void OpenCreateFormatDialog()
    {
        FormatFormModal.Open(null);
    }

    private void OpenEditFormatDialog(FormatDto format)
    {
        FormatFormModal.Open(format);
    }

    private async Task ConfirmDeleteFormat(FormatDto format)
    {
        var message = format.IsActive ? "Deactivate" : "Activate";
        
        var result = await DialogService.ShowMessageBox(
            "Activation Status Confirmation",
            $"Are you sure you want to {message.ToLower()} '{format.Title}'?",
            message,
            "Cancel"
        );

        if (result == true)
        {
            await DeleteFormat(format.Id);
        }
    }

    private async Task DeleteFormat(Guid id)
    {
        try
        {
            await ApiClient.UpdateFormatStatusAsync(id);

            await LoadFormats();
            
            Snackbar.Add("Format status updated successfully", Severity.Success);
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