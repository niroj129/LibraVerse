using LibraVerse.WASM.Services;
using MudBlazor;

namespace LibraVerse.WASM.Pages.Wishlist;

public partial class Wishlist
{
    private List<WishlistDto>? wishlistItems;
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadWishlistItems();
    }

    private async Task LoadWishlistItems()
    {
        isLoading = true;
        StateHasChanged();

        try
        {
            wishlistItems = (await ApiClient.GetAllWishlistsListAsync()).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load wishlist items: " + ex.Message, Severity.Error);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task AddToCart(WishlistDto item)
    {
        try
        {
            var request = new CreateCartDto()
            {
                BookId = item.Book.Id
            };

            var response = await ApiClient.AddToCartAsync(request);

            Snackbar.Add($"\"{item.Book.Title}\" added to cart", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add("Error: " + ex.Message, Severity.Error);
        }
    }

    private async Task RemoveFromWishlist(WishlistDto item)
    {
        try
        {
            await ApiClient.DeleteWishlistAsync(item.Id);

            wishlistItems?.Remove(item);
            Snackbar.Add("Item removed from wishlist", Severity.Success);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Add("Error: " + ex.Message, Severity.Error);
        }
    }

    private async Task ConfirmClearWishlist()
    {
        var result = await DialogService.ShowMessageBox(
            "Clear Wishlist",
            "Are you sure you want to remove all items from your wishlist?",
            yesText: "Yes, clear wishlist",
            noText: "Cancel");

        if (result == true)
        {
            await ClearWishlist();
        }
    }

    private async Task ClearWishlist()
    {
        if (wishlistItems == null || wishlistItems.Count == 0) return;

        isLoading = true;
        StateHasChanged();

        try
        {
            foreach (var item in wishlistItems.ToList())
            {
                await ApiClient.DeleteWishlistAsync(item.Id);
            }

            wishlistItems.Clear();
            Snackbar.Add("Wishlist cleared successfully", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add("Error: " + ex.Message, Severity.Error);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task AddAllToCart()
    {
        if (wishlistItems == null || wishlistItems.Count == 0) return;

        isLoading = true;
        StateHasChanged();

        try
        {
            int successCount = 0;

            foreach (var item in wishlistItems)
            {
                if (item.Book.IsAvailable)
                {
                    var request = new CreateCartDto
                    {
                        BookId = item.Book.Id
                    };

                    await ApiClient.AddToCartAsync(request);

                    successCount++;
                }
            }

            if (successCount > 0)
            {
                Snackbar.Add($"{successCount} items added to cart", Severity.Success);
            }
            else
            {
                Snackbar.Add("No available items to add to cart", Severity.Warning);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add("Error: " + ex.Message, Severity.Error);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private void NavigateToBooks()
    {
        NavigationManager.NavigateTo("/books");
    }
}