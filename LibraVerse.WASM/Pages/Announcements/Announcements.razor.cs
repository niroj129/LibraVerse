using LibraVerse.WASM.Services;
using MudBlazor;

namespace LibraVerse.WASM.Pages.Announcements;

public partial class Announcements
{
    private List<AnnouncementDto>? allAnnouncements;
    private List<AnnouncementDto>? filteredAnnouncements;
    private bool isLoading = true;
    private int currentPage = 1;
    private int pageSize = 5;
    private int totalPages = 1;
    private string statusFilter = "";
    private List<string> selectedTypes = new();
    private List<string> availableTypes = new();

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
            allAnnouncements = (await ApiClient.GetAllAnnouncementsListAsync()).ToList();

            if (allAnnouncements != null)
            {
                availableTypes = allAnnouncements
                    .Select(a => a.Type)
                    .Distinct()
                    .ToList();

                ApplyFilters();
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

    private void ApplyFilters()
    {
        if (allAnnouncements == null) return;

        var filtered = allAnnouncements.AsEnumerable();

        // Apply type filter
        if (selectedTypes.Count > 0)
        {
            filtered = filtered.Where(a => selectedTypes.Contains(a.Type));
        }

        // Apply status filter
        if (!string.IsNullOrEmpty(statusFilter))
        {
            var now = DateTime.Now;

            switch (statusFilter.ToLower())
            {
                case "active":
                    filtered = filtered.Where(a => now >= a.StartDate && now <= a.EndDate);
                    break;
                case "upcoming":
                    filtered = filtered.Where(a => now < a.StartDate);
                    break;
            }
        }

        // Sort by date (newest first)
        filtered = filtered.OrderByDescending(a => a.StartDate);

        var totalItems = filtered.Count();
        totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        // Apply pagination
        filtered = filtered
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize);

        filteredAnnouncements = filtered.ToList();
    }

    private void ChangePage(int page)
    {
        currentPage = page;
        ApplyFilters();
    }

    private void ToggleType(string type)
    {
        if (selectedTypes.Contains(type))
        {
            selectedTypes.Remove(type);
        }
        else
        {
            selectedTypes.Add(type);
        }

        currentPage = 1;
        ApplyFilters();
    }

    private void ToggleAllTypes()
    {
        if (selectedTypes.Count == 0)
        {
            selectedTypes = new List<string>(availableTypes);
        }
        else
        {
            selectedTypes.Clear();
        }

        currentPage = 1;
        ApplyFilters();
    }

    private void SetStatusFilter(string status)
    {
        statusFilter = status;
        currentPage = 1;
        ApplyFilters();
    }

    private void ResetFilters()
    {
        selectedTypes.Clear();
        statusFilter = "";
        currentPage = 1;
        ApplyFilters();
    }

    private void NavigateToBook(Guid bookId)
    {
        NavigationManager.NavigateTo($"/books/{bookId}/details");
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