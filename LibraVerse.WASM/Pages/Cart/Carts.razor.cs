using LibraVerse.WASM.Services;
using MudBlazor;

namespace LibraVerse.WASM.Pages.Cart;

public partial class Carts
{
    private List<CartDto>? cartItems;
    private bool isLoading = true;
    private bool isProcessing = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadCartItems();
    }

    private async Task LoadCartItems()
    {
        isLoading = true;
        StateHasChanged();

        try
        {
            cartItems = (await ApiClient.GetAllCartListAsync()).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task IncreaseQuantity(CartDto item)
    {
        try
        {
            var request = new CreateCartDto()
            {
                BookId = item.Book.Id
            };

            await ApiClient.AddToCartAsync(request);

            item.Quantity++;
            item.TotalPrice = item.Book.Price * item.Quantity;
            StateHasChanged();
            
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private async Task DecreaseQuantity(CartDto item)
    {
        if (item.Quantity <= 1) return;

        try
        {
            // Find the cart ID from the response
            await ApiClient.RemoveFromCartAsync(item.Id, item.Book.Id);
            item.Quantity--;
            item.TotalPrice = item.Book.Price * item.Quantity;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }


    private async Task RemoveFromCart(CartDto item)
    {
        try
        {
            // Find the cart ID from the response
            await ApiClient.RemoveCartAsync(item.Id);
            cartItems?.Remove(item);
            Snackbar.Add("Item removed from cart", Severity.Success);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private async Task ConfirmClearCart()
    {
        var result = await DialogService.ShowMessageBox(
            "Clear Cart",
            "Are you sure you want to remove all items from your cart?",
            yesText: "Yes, clear cart",
            noText: "Cancel");

        if (result == true)
        {
            await ClearCart();
        }
    }

    private async Task ClearCart()
    {
        if (cartItems == null || cartItems.Count == 0) return;

        isLoading = true;
        StateHasChanged();

        try
        {
            bool success = true;

            if (success)
            {
                cartItems.Clear();
                Snackbar.Add("Cart cleared successfully", Severity.Success);
            }
            else
            {
                Snackbar.Add("Failed to clear some items from cart", Severity.Warning);
                await LoadCartItems();
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private double CalculateSubtotal()
    {
        if (cartItems == null) return 0;

        return cartItems.Sum(item => item.Book.Price * item.Quantity);
    }

    private double CalculateDiscount()
    {
        if (cartItems == null) return 0;

        double discount = 0;

        foreach (var item in cartItems)
        {
            if (item.Book.Discount != null && item.Book.Discount.MarkAsSale &&
                item.Book.Discount.EndDate > DateTime.Now)
            {
                discount += item.Book.Price * item.Book.Discount.DiscountPercentage / (double)100m * item.Quantity;
            }
        }

        return discount;
    }

    private double CalculateTotal()
    {
        return CalculateSubtotal() - CalculateDiscount();
    }

    private async Task PlaceOrder()
    {
        if (cartItems == null || cartItems.Count == 0) return;

        isProcessing = true;
        StateHasChanged();

        try
        {
            await ApiClient.CheckoutOrderAsync();
            Snackbar.Add("Order placed successfully!", Severity.Success);
            cartItems.Clear();
            NavigationManager.NavigateTo("/assigned/orders");
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            isProcessing = false;
            StateHasChanged();
        }
    }

    private void NavigateToBooks()
    {
        NavigationManager.NavigateTo("/books");
    }
}