using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LibraVerse.WASM.Pages.Orders;

public partial class MyOrders
{
    private OrderDtoPaginatedResponse? orders;
    private bool isLoading = true;
    private int currentPage = 1;
    private int pageSize = 10;
    private string statusFilter = "";
    private bool isOrderDetailsModalOpen = false;
    private OrderDto? selectedOrder;
    private string _selectedUser = "";
    private string _claimCode = "";
    
    protected override async Task OnInitializedAsync()
    {
        await LoadOrders();
    }

    private async Task LoadOrders()
    {
        isLoading = true;
        StateHasChanged();

        try
        {
            orders = await ApiClient.GetAllMyOrdersAsync(currentPage, pageSize, _orderStatus);
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load orders: " + ex.Message, Severity.Error);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private string _activeFilter = "";
    private OrderStatus? _orderStatus = null;
    private async Task HandleStatusChange(ChangeEventArgs e)
    {
        if (e.Value != null)
        {
            _activeFilter = e.Value.ToString();
            _orderStatus = _activeFilter switch
            {
                "Pending" => OrderStatus._1,
                "Completed" => OrderStatus._2,
                "Cancelled" => OrderStatus._3,
                _ => null
            };
        }

        await LoadOrders();
    }

    private async Task ChangePage(int page)
    {
        currentPage = page;
        await LoadOrders();
    }

    private void ShowOrderDetails(OrderDto order)
    {
        selectedOrder = order;
        _claimCode = string.Empty;
        isOrderDetailsModalOpen = true;
        StateHasChanged();
    }

    private void CloseOrderDetailsModal()
    {
        isOrderDetailsModalOpen = false;
        StateHasChanged();
    }

    private async Task ConfirmCancelOrder(OrderDto order)
    {
        var result = await DialogService.ShowMessageBox(
            "Cancel Order",
            "Are you sure you want to cancel this order?",
            yesText: "Yes, cancel order",
            noText: "No, keep order");

        if (result == true)
        {
            await CancelOrder(order);
        }
    }

    private async Task CancelOrder(OrderDto order)
    {
        try
        {
            await ApiClient.CancelOrderAsync(order.Id);

            Snackbar.Add("Order cancelled successfully", Severity.Success);

            isOrderDetailsModalOpen = false;
            
            await LoadOrders();
        }
        catch (Exception ex)
        {
            Snackbar.Add("Error: " + ex.Message, Severity.Error);
        }
    }

    private void NavigateToBooks()
    {
        NavigationManager.NavigateTo("/books");
    }

    private void NavigateToCart()
    {
        NavigationManager.NavigateTo("/cart");
    }
}