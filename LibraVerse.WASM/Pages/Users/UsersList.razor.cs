using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LibraVerse.WASM.Pages.Users;

public partial class UsersList
{
    private UserDtoPaginatedResponse? _users;
    private bool _isLoading = true;
    private string _searchTerm = "";
    private string? _activeFilter = "";
    private string? _roleFilter = "";
    private int _currentPage = 1;
    private readonly int _pageSize = 8;
    private string _viewMode = "grid";
    private UserFormModal UserFormModal;
    private CreateUserDto? userFormModal;
    private System.Timers.Timer? _searchTimer;
    private bool? _active;
    private Role? _role;
    
    protected override async Task OnInitializedAsync()
    {
        _searchTimer = new System.Timers.Timer(500);
        _searchTimer.Elapsed += async (sender, e) => await SearchUsers();
        _searchTimer.AutoReset = false;

        await LoadUsers();
    }

    private async Task HandleStatusChange(ChangeEventArgs e)
    {
        if (e.Value != null)
        {
            _activeFilter = e.Value.ToString();
            _active = _activeFilter == "Active" ? true : _activeFilter == "Inactive" ? false : null;
        }

        await LoadUsers();
    }
    
    private async Task HandleRoleChange(ChangeEventArgs e)
    {
        if (e.Value != null)
        {
            _roleFilter = e.Value.ToString();
            _role = _roleFilter switch
            {
                "Admin" => Role._1,
                "Staff" => Role._2,
                "Customer" => Role._3,
                _ => null
            };
        }

        await LoadUsers();
    }
    
    private async Task LoadUsers()
    {
        _isLoading = true;
        StateHasChanged();

        try
        {
            _users = await ApiClient.GetAllUsersAsync(_currentPage, _pageSize, _searchTerm, _active, _role);
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load users: " + ex.Message, Severity.Error);
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

    private async Task SearchUsers()
    {
        await InvokeAsync(async () =>
        {
            _currentPage = 1;
            await LoadUsers();
        });
    }

    private async Task ChangePage(int page)
    {
        _currentPage = page;
        await LoadUsers();
    }

    private void OpenCreateUserDialog()
    {
        UserFormModal.Open(null);
    }

    private void OpenEditUserDialog(UserDto user)
    {
        UserFormModal.Open(user);
    }

    private async Task ConfirmDeleteUser(UserDto user)
    {
        var message = user.IsActive ? "Deactivate" : "Activate";
        
        var result = await DialogService.ShowMessageBox(
            "Activation Status Confirmation",
            $"Are you sure you want to {message.ToLower()} '{user.Name}'?",
            message,
            "Cancel"
        );

        if (result == true)
        {
            await DeleteUser(user.Id);
        }
    }

    private async Task DeleteUser(Guid id)
    {
        try
        {
            await ApiClient.UpdateUserActivationStatusAsync(id);

            await LoadUsers();
            
            Snackbar.Add("User status updated successfully", Severity.Success);
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