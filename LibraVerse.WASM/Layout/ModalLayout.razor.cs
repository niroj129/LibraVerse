using Microsoft.AspNetCore.Components;

namespace LibraVerse.WASM.Layout;

public partial class ModalLayout
{
    [Parameter] public string Title { get; set; } = "Modal";
    
    [Parameter] public bool IsVisible { get; set; }
    
    [Parameter] public string Size { get; set; } = "";
    
    [Parameter] public EventCallback OnClose { get; set; }
    
    [Parameter] public RenderFragment? ChildContent { get; set; }
    
    [Parameter] public RenderFragment? FooterContent { get; set; }

    private async Task Close()
    {
        await OnClose.InvokeAsync();
    }
}