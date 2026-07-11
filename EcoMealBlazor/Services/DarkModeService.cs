namespace EcoMeal.EcoMealBlazor.Services;

public class DarkModeService
{
    public event Action? OnChange;
    private bool _isDarkMode = false;

    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            if (_isDarkMode != value)
            {
                _isDarkMode = value;
                NotifyStateChanged();
            }
        }
    }

    public void Toggle()
    {
        IsDarkMode = !IsDarkMode;
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}