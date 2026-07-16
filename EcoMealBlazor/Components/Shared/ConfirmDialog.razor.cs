using Microsoft.AspNetCore.Components;

namespace EcoMeal.Components.Shared;

public class ConfirmDialogBase : ComponentBase
{
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public string Message { get; set; } = "Are you sure?";
    [Parameter] public string ConfirmText { get; set; } = "Confirm";
    [Parameter] public string CancelText { get; set; } = "Cancel";
    [Parameter] public EventCallback OnConfirm { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
}
