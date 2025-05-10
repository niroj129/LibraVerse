using LibraVerse.WASM.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LibraVerse.WASM.Pages.Books;

public partial class ReviewModal
{
    [Parameter] public EventCallback OnReviewAdded { get; set; }
    [Parameter] public Guid BookId { get; set; }
    private bool isOpen = false;
    private string reviewText = string.Empty;
    private int rating = 0;

    public void Open(Guid bookId)
    {
        BookId = bookId;
        isOpen = true;
        reviewText = string.Empty;
        rating = 0;
        StateHasChanged();
    }

    private void Close()
    {
        isOpen = false;
    }

    private async Task SubmitReview()
    {
        if (rating == 0 || string.IsNullOrWhiteSpace(reviewText))
        {
            Snackbar.Add("Please enter a rating and a review message.", Severity.Warning);
            return;
        }

        try
        {
            var review = new CreateReviewDto
            {
                BookId = BookId,
                Text = reviewText,
                Rating = rating
            };

            await ApiClient.CreateReviewAsync(review);
            Snackbar.Add("Review added successfully!", Severity.Success);

            Close();
            await OnReviewAdded.InvokeAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add("Error: " + ex.Message, Severity.Error);
        }
    }
}