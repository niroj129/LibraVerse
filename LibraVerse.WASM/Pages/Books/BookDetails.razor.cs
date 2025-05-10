using System.Net.Http.Json;
using System.Security.Claims;
using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LibraVerse.WASM.Pages.Books;

public partial class BookDetails
{
    [Parameter] public Guid BookId { get; set; }

    private BookDto? book;
    private bool isLoading = true;
    private BookFormModal? bookFormModal;

    private string? _role = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadBook();
        
        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        
        _role  = authenticationState.User.FindFirst(c => c.Type == ClaimTypes.Role)?.Value;
    }

    protected override async Task OnParametersSetAsync()
    {
        await LoadBook();
    }

    private async Task LoadBook()
    {
        isLoading = true;
        StateHasChanged();

        try
        {
            var books = await ApiClient.GetAllBooksListAsync(null, null, null, null, null);
            book = books?.FirstOrDefault(b => b.Id == BookId);
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load book: " + ex.Message, Severity.Error);
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

    private void OpenEditBookDialog(BookDto book)
    {
        bookFormModal?.Open(book);
    }

    private async Task ToggleAvailability()
    {
        if (book == null) return;

        try
        {
            await ApiClient.UpdateBookAvailabilityStatusAsync(book.Id);
            
            Snackbar.Add("Book availability updated successfully", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add("Error: " + ex.Message, Severity.Error);
        }
    }
    
    private async Task ConfirmBookWishlist(BookDto bookModel)
    {
        var result = await DialogService.ShowMessageBox(
            "Wishlist Confirmation",
            $"Are you sure you want to add '{bookModel.Title}' to your wishlist?",
            "Add to Wishlist",
            "Cancel"
        );

        if (result == true)
        {
            await AddToWishList(bookModel.Id);
        }
    }

    private async Task AddToWishList(Guid bookId)
    {
        try
        {
            var model = new CreateWishlistDto()
            {
                BookId = bookId
            };
            
            await ApiClient.CreateWishlistAsync(model);
            
            Snackbar.Add("Book successfully added to wishlist", Severity.Success);
            
            await LoadBook();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
    
    private ReviewModal? reviewModal;

    private void OpenReviewModal()
    {
        reviewModal?.Open(book?.Id ?? Guid.Empty);
    }
    
    private async Task ConfirmBookCart(BookDto bookModel)
    {
        var message = bookModel.IsAddedToCart ? $"Are you sure you want to add '{bookModel.Title}' to your cart? You have already added the respective book to your cart." : $"Are you sure you want to add '{bookModel.Title}' to your cart?";
     
        var result = await DialogService.ShowMessageBox(
            "Cart Confirmation",
            message,
            "Add to Cart",
            "Cancel"
        );

        if (result == true)
        {
            await AddToCart(bookModel.Id);
        }
    }

    private async Task AddToCart(Guid bookId)
    {
        try
        {
            var model = new CreateCartDto()
            {
                BookId = bookId
            };
            
            await ApiClient.AddToCartAsync(model);
            
            Snackbar.Add("Book successfully added to cart", Severity.Success);
            
            await LoadBook();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
    
    private DiscountFormModal? discountFormModal;
    
    private void OpenCreateDiscountModal()
    {
        if (book != null)
        {
            discountFormModal?.Open(book.Id);
        }
    }

    private void OpenEditDiscountModal(DiscountDto discount)
    {
        if (book != null)
        {
            discountFormModal?.Open(book.Id, discount);
        }
    }
}