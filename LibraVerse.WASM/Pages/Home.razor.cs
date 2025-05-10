using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace LibraVerse.WASM.Pages;

public partial class Home
{
    private BookDtoPaginatedResponse? books;
    private List<BookDto>? featuredBooks;
    private List<FormatDto>? formats;
    private List<PublicationDto>? publications;
    private bool isLoadingBooks = true;
    private bool isLoadingFormats = true;
    private bool isLoadingPublications = true;
    private int currentPage = 1;
    private int pageSize = 9;
    private string searchQuery = "";
    private string currentTab = "all";
    private string viewMode = "grid";
    private List<Guid> selectedFormatIds = new();
    private List<Guid> selectedPublicationIds = new();
    private bool? availabilityFilter = null;
    private decimal? minPrice;
    private decimal? maxPrice;
    private List<string> popularGenres = new() { "Science Fiction", "Non Fiction", "Adventure", "Biography", "Thriller", "Children", "Romance", "Fiction", "Fantasy" };

    private class TabInfo
    {
        public string Key { get; set; } = "";
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "";
    }

    private List<TabInfo> tabs = new()
    {
        new TabInfo { Key = "all", Name = "All Books", Icon = "bi-book" },
        new TabInfo { Key = "bestsellers", Name = "Bestsellers", Icon = "bi-star" },
        new TabInfo { Key = "new-releases", Name = "New Releases", Icon = "bi-calendar-check" },
        new TabInfo { Key = "new-arrivals", Name = "New Arrivals", Icon = "bi-box-seam" },
        new TabInfo { Key = "coming-soon", Name = "Coming Soon", Icon = "bi-hourglass-split" },
        new TabInfo { Key = "deals", Name = "Deals", Icon = "bi-tag" }
    };

    protected override async Task OnInitializedAsync()
    {
        await LoadFormats();
        await LoadPublications();
        await LoadBooks();
        await LoadFeaturedBooks();
    }

    private async Task LoadBooks()
    {
        isLoadingBooks = true;
        StateHasChanged();

        try
        {
            books = await ApiClient.GetAllBooksAsync(currentPage, pageSize, searchQuery, true, selectedFormatIds.FirstOrDefault(), selectedPublicationIds.FirstOrDefault(), availabilityFilter);

            books.Items = currentTab switch
            {
                "bestsellers" => books.Items.Where(x => x.Reviews.Count >= 2).OrderByDescending(x => x.Reviews).ToList(),
                "new-releases" => books.Items.Where(x => x.PublishedDate >= DateTimeOffset.Now.AddMonths(-3)).OrderByDescending(x => x.PublishedDate).ToList(),
                "new-arrivals" => books.Items.Where(x => x.PublishedDate >= DateTimeOffset.Now.AddMonths(-1)).OrderByDescending(x => x.PublishedDate).ToList(),
                "coming-soon" => books.Items.Where(x => x.PublishedDate >= DateTimeOffset.Now.AddMonths(1)).OrderByDescending(x => x.PublishedDate).ToList(),
                "deals" => books.Items.Where(x => x.Discount is { MarkAsSale: true } && x.Discount.EndDate > DateTime.Now).OrderByDescending(x => x.Discount.DiscountPercentage).ToList(),
                _ => books.Items
            };
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load books: " + ex.Message, Severity.Error);
        }
        finally
        {
            isLoadingBooks = false;
            StateHasChanged();
        }
    }

    private async Task LoadFeaturedBooks()
    {
        try
        {
            var books = await ApiClient.GetAllBooksAsync(1, 5, null, true, null, null, availabilityFilter);
            featuredBooks = books?.Items.ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load featured books: " + ex.Message, Severity.Error);
        }
    }

    private async Task LoadFormats()
    {
        isLoadingFormats = true;
        StateHasChanged();

        try
        {
            formats = (await ApiClient.GetAllFormatsListAsync(null, true)).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load formats: " + ex.Message, Severity.Error);
        }
        finally
        {
            isLoadingFormats = false;
            StateHasChanged();
        }
    }

    private async Task LoadPublications()
    {
        isLoadingPublications = true;
        StateHasChanged();

        try
        {
            publications = (await ApiClient.GetAllPublicationsListAsync(null, true)).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load publications: " + ex.Message, Severity.Error);
        }
        finally
        {
            isLoadingPublications = false;
            StateHasChanged();
        }
    }

    private async Task ChangePage(int page)
    {
        currentPage = page;
        await LoadBooks();
    }

    private async Task ChangeTab(string tab)
    {
        currentTab = tab;
        currentPage = 1;
        await LoadBooks();
    }

    private async Task ApplySearch()
    {
        currentPage = 1;
        await LoadBooks();
    }

    private async Task HandleSearchKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await ApplySearch();
        }
    }

    private async Task ToggleFormat(Guid formatId)
    {
        if (selectedFormatIds.Contains(formatId))
        {
            selectedFormatIds.Remove(formatId);
        }
        else
        {
            selectedFormatIds.Add(formatId);
        }
        
        currentPage = 1;
        await LoadBooks();
    }

    private async Task TogglePublication(Guid publicationId)
    {
        if (selectedPublicationIds.Contains(publicationId))
        {
            selectedPublicationIds.Remove(publicationId);
        }
        else
        {
            selectedPublicationIds.Add(publicationId);
        }
        
        currentPage = 1;
        await LoadBooks();
    }

    private async Task SetAvailabilityFilter(bool? value)
    {
        availabilityFilter = value;
        currentPage = 1;
        await LoadBooks();
    }

    private async Task ApplyPriceFilter()
    {
        currentPage = 1;
        await LoadBooks();
    }

    private async Task ApplySorting()
    {
        currentPage = 1;
        await LoadBooks();
    }

    private void SetViewMode(string mode)
    {
        viewMode = mode;
    }

    private async Task ResetFilters()
    {
        searchQuery = "";
        selectedFormatIds.Clear();
        selectedPublicationIds.Clear();
        availabilityFilter = null;
        minPrice = null;
        maxPrice = null;
        currentPage = 1;
        await LoadBooks();
    }

    private async Task QuickFilterByGenre(string genre)
    {
        searchQuery = genre;
        await ApplySearch();
    }

    private void NavigateToBookDetails(Guid bookId)
    {
        NavigationManager.NavigateTo($"/books/{bookId}/details");
    }

    private async Task AddToCart(BookDto book)
    {
        if (!book.IsAvailable)
        {
            Snackbar.Add("This book is currently out of stock", Severity.Warning);
            return;
        }

        try
        {
            var request = new CreateCartDto()
            {
                BookId = book.Id
            };

            await ApiClient.AddToCartAsync(request);
            Snackbar.Add($"\"{book.Title}\" added to cart", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add("Error: " + ex.Message, Severity.Error);
        }
    }

    private async Task AddToWishlist(BookDto book)
    {
        try
        {
            if (book.IsAddedToWishlist)
            {
                Snackbar.Add($"You have already added this book on your wishlist, to remove it, please go to the wishlist page.", Severity.Info);
            }
            else
            {
                var request = new CreateWishlistDto()
                {
                    BookId = book.Id
                };
                
                await ApiClient.CreateWishlistAsync(request);
                Snackbar.Add($"\"{book.Title}\" added to wishlist", Severity.Success);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add("Error: " + ex.Message, Severity.Error);
        }
    }
}