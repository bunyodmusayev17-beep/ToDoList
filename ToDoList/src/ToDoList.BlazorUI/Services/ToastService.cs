namespace ToDoList.BlazorUI.Services;

public enum ToastLevel
{
    Success,
    Error,
    Info,
    Warning
}

public record ToastMessage(Guid Id, string Text, ToastLevel Level);

public class ToastService
{
    private readonly List<ToastMessage> _toasts = new();

    public event Action? OnChange;

    public IReadOnlyList<ToastMessage> Toasts => _toasts;

    public void ShowSuccess(string text) => Show(text, ToastLevel.Success);
    public void ShowError(string text) => Show(text, ToastLevel.Error);
    public void ShowInfo(string text) => Show(text, ToastLevel.Info);
    public void ShowWarning(string text) => Show(text, ToastLevel.Warning);

    public void Show(string text, ToastLevel level = ToastLevel.Info)
    {
        var toast = new ToastMessage(Guid.NewGuid(), text, level);
        _toasts.Add(toast);
        OnChange?.Invoke();
        _ = RemoveAfterDelayAsync(toast.Id);
    }

    public void Remove(Guid id)
    {
        _toasts.RemoveAll(t => t.Id == id);
        OnChange?.Invoke();
    }

    private async Task RemoveAfterDelayAsync(Guid id)
    {
        await Task.Delay(4000);
        Remove(id);
    }
}
