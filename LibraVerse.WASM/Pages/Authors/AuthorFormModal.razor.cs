using MudBlazor;
using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components;

namespace LibraVerse.WASM.Pages.Authors;

public partial class AuthorFormModal
{
    [Parameter] public EventCallback OnSave { get; set; }

    private bool _isOpen;
    
    private bool _isEditMode;
    
    private bool _isSaving;
    
    private Guid? _authorId;
    
    private CreateAuthorDto _authorModel = new();

    public void Open(AuthorDto? author)
    {
        if (author != null)
        {
            _isEditMode = true;
            _authorId = author.Id;
            _authorModel = new CreateAuthorDto
            {
                Name = author.Name,
                Email = author.Email,
                Biography = author.Biography
            };
        }
        else
        {
            _isEditMode = false;
            _authorId = null;
            _authorModel = new CreateAuthorDto();
        }

        _isOpen = true;
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
            if (_isEditMode && _authorId.HasValue)
            {
                var updateModel = new UpdateAuthorDto()
                {
                    Name = _authorModel.Name,
                    Email = _authorModel.Email,
                    Biography = _authorModel.Biography
                };

                await ApiClient.UpdateAuthorAsync(_authorId ?? Guid.Empty, updateModel);
            }
            else
            {
                await ApiClient.CreateAuthorAsync(_authorModel);
            }

            Snackbar.Add($"Author {(_isEditMode ? "updated" : "created")} successfully", Severity.Success);

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