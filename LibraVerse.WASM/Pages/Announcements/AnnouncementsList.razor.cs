using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LibraVerse.WASM.Pages.Announcements;

public partial class AnnouncementsList
{
    private AnnouncementDtoPaginatedResponse? announcements;
    private List<AnnouncementDto> filteredAnnouncements = new();
    private bool isLoading = true;
    private int currentPage = 1;
    private int pageSize = 10;
    private string typeFilter = "";
    private string statusFilter = "";
    private AnnouncementFormModal? announcementFormModal;

    protected override async Task OnInitializedAsync()
    {
        await LoadAnnouncements();
    }

    private async Task LoadAnnouncements()
    {
        isLoading = true;
        StateHasChanged();

        try
        {
            announcements = await ApiClient.GetAllAnnouncementsAsync(currentPage, pageSize);

            if (announcements?.Items != null)
            {
                ApplyFiltersInternal();
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load announcements: " + ex.Message, Severity.Error);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private void ApplyFiltersInternal()
    {
        if (announcements?.Items == null) return;

        filteredAnnouncements = announcements.Items.ToList();

        if (!string.IsNullOrEmpty(typeFilter))
        {
            if (Enum.TryParse<AnnouncementType>(typeFilter, out var type))
            {
                filteredAnnouncements = filteredAnnouncements.Where(a => a.Type == type.ToString()).ToList();
            }
        }

        if (!string.IsNullOrEmpty(statusFilter))
        {
            var now = DateTime.Now;

            switch (statusFilter.ToLower())
            {
                case "active":
                    filteredAnnouncements = filteredAnnouncements.Where(a => now >= a.StartDate && now <= a.EndDate)
                        .ToList();
                    break;
                case "upcoming":
                    filteredAnnouncements = filteredAnnouncements.Where(a => now < a.StartDate).ToList();
                    break;
                case "expired":
                    filteredAnnouncements = filteredAnnouncements.Where(a => now > a.EndDate).ToList();
                    break;
            }
        }
    }

    private async Task HandleTypeChange(ChangeEventArgs e)
    {
        if (e.Value != null)
        {
            typeFilter = e.Value.ToString() ?? "";
        }

        await LoadAnnouncements();
    }

    private async Task HandleStatusChange(ChangeEventArgs e)
    {
        if (e.Value != null)
        {
            statusFilter = e.Value.ToString() ?? "";
        }

        await LoadAnnouncements();
    }

    private async Task ChangePage(int page)
    {
        currentPage = page;
        await LoadAnnouncements();
    }

    private async Task OpenCreateAnnouncementModal()
    {
        if (announcementFormModal != null)
        {
            await announcementFormModal.Open();
        }
    }

    private async Task OpenEditAnnouncementModal(AnnouncementDto announcement)
    {
        if (announcementFormModal != null)
        {
            await announcementFormModal.Open(announcement);
        }
    }

    private async Task ConfirmDeleteAnnouncement(AnnouncementDto announcement)
    {
        var result = await DialogService.ShowMessageBox(
            "Delete Announcement",
            $"Are you sure you want to delete the announcement \"{announcement.Title}\"?",
            yesText: "Yes, delete it",
            noText: "Cancel");

        if (result == true)
        {
            await DeleteAnnouncement(announcement);
        }
    }

    private async Task DeleteAnnouncement(AnnouncementDto announcement)
    {
        try
        {
            var response = await ApiClient.DeleteAnnouncementAsync(announcement.Id);

            Snackbar.Add("Announcement deleted successfully", Severity.Success);
            await LoadAnnouncements();
        }
        catch (Exception ex)
        {
            Snackbar.Add("Error: " + ex.Message, Severity.Error);
        }
    }

    private string GetAnnouncementTypeIcon(string type)
    {
        return type switch
        {
            "Offer" => "bi-tag",
            "News" => "bi-newspaper",
            "Event" => "bi-calendar-event",
            "Update" => "bi-arrow-clockwise",
            "Alert" => "bi-exclamation-triangle",
            _ => "bi-megaphone"
        };
    }

    private string GetAnnouncementTypeClass(string type)
    {
        return type switch
        {
            "Offer" => "bg-success",
            "News" => "bg-primary",
            "Event" => "bg-warning",
            "Update" => "bg-info",
            "Alert" => "bg-danger",
            _ => "bg-secondary"
        };
    }

    private string GetAnnouncementTypeBadgeClass(string type)
    {
        return type switch
        {
            "Offer" => "bg-success",
            "News" => "bg-primary",
            "Event" => "bg-warning",
            "Update" => "bg-info",
            "Alert" => "bg-danger",
            _ => "bg-secondary"
        };
    }
}