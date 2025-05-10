using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LibraVerse.WASM.Pages.Users;

public partial class UserFormModal
{
    [Parameter] public EventCallback OnSave { get; set; }

    private bool _isOpen;
    
    private bool _isEditMode;
    
    private bool _isSaving;
    
    private CreateUserDto _userModel = new();

    public void Open(UserDto? user)
    {
        _isOpen = true;
        _isEditMode = false;
        _userModel = new CreateUserDto()
        {
            Role = Role._2
        };

        StateHasChanged();
    }

    private void Close()
    {
        _isOpen = false;

        StateHasChanged();
    }

    private async Task HandleSubmit()
    {
        _isSaving = true;
        StateHasChanged();

        try
        {
            await ApiClient.RegisterUserAsync(_userModel);

            Snackbar.Add("User registered successfully.", Severity.Success);

            Close();

            await OnSave.InvokeAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            _isSaving = false;
            StateHasChanged();
        }
    }
}