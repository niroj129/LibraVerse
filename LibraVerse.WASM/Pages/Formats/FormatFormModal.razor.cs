using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LibraVerse.WASM.Pages.Formats;

public partial class FormatFormModal
{
    [Parameter] public EventCallback OnSave { get; set; }

    private bool _isOpen;
    
    private bool _isEditMode;
    
    private bool _isSaving;
    
    private Guid? _formatId;
    
    private CreateFormatDto _formatModel = new();

    public void Open(FormatDto? format)
    {
        if (format != null)
        {
            _isEditMode = true;
            _formatId = format.Id;
            _formatModel = new CreateFormatDto
            {
                Title = format.Title,
                Description = format.Description
            };
        }
        else
        {
            _isEditMode = false;
            _formatId = null;
            _formatModel = new CreateFormatDto();
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
            if (_isEditMode && _formatId.HasValue)
            {
                var updateModel = new UpdateFormatDto()
                {
                    Title = _formatModel.Title,
                    Description = _formatModel.Description
                };

                await ApiClient.UpdateFormatAsync(_formatId ?? Guid.Empty, updateModel);
            }
            else
            {
                await ApiClient.CreateFormatAsync(_formatModel);
            }

            Snackbar.Add($"Format {(_isEditMode ? "updated" : "created")} successfully", Severity.Success);

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