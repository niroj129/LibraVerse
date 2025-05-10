using MudBlazor;
using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components;

namespace LibraVerse.WASM.Pages.Publications;

public partial class PublicationFormModal
{
    [Parameter] public EventCallback OnSave { get; set; }

    private bool _isOpen;
    
    private bool _isEditMode;
    
    private bool _isSaving;
    
    private Guid? _publicationId;
    
    private CreatePublicationDto _publicationModel = new();

    public void Open(PublicationDto? publication)
    {
        if (publication != null)
        {
            _isEditMode = true;
            _publicationId = publication.Id;
            _publicationModel = new CreatePublicationDto
            {
                Title = publication.Title,
                Description = publication.Description
            };
        }
        else
        {
            _isEditMode = false;
            _publicationId = null;
            _publicationModel = new CreatePublicationDto();
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
            if (_isEditMode && _publicationId.HasValue)
            {
                var updateModel = new UpdatePublicationDto()
                {
                    Title = _publicationModel.Title,
                    Description = _publicationModel.Description
                };

                await ApiClient.UpdatePublicationAsync(_publicationId ?? Guid.Empty, updateModel);
            }
            else
            {
                await ApiClient.CreatePublicationAsync(_publicationModel);
            }

            Snackbar.Add($"Publication {(_isEditMode ? "updated" : "created")} successfully", Severity.Success);

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