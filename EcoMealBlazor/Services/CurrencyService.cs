using System.Globalization;

namespace EcoMeal.EcoMealBlazor.Services;

// converts RON prices based on thread culture
public class CurrencyService
{
    private static readonly Dictionary<string, (decimal Rate, string Symbol)> Rates = new()
    {
        ["en-US"] = (0.220m, "$"),
        ["en-GB"] = (0.174m, "\u00a3"),
        ["ro-RO"] = (1.0m, "lei"),
        ["de-DE"] = (0.201m, "\u20ac"),
        ["fr-FR"] = (0.201m, "\u20ac"),
        ["sr-RS"] = (23.50m, "RSD"),
        ["fi-FI"] = (0.201m, "\u20ac"),
        ["ja-JP"] = (34.0m, "\u00a5"),
        ["ar-SA"] = (0.825m, "\u0631.\u0633"),
    };

    public (decimal Converted, string Symbol) Convert(decimal priceInRon)
    {
        var culture = CultureInfo.CurrentCulture.Name;
        if (Rates.TryGetValue(culture, out var rate))
            return (Math.Round(priceInRon * rate.Rate, 2), rate.Symbol);
        return (priceInRon, "lei");
    }

    public string Format(decimal priceInRon)
    {
        var (converted, symbol) = Convert(priceInRon);
        return $"{converted:F2} {symbol}";
    }

    public string Format(double priceInRon) => Format((decimal)priceInRon);

    public (decimal Converted, string Symbol) Convert(double priceInRon) => Convert((decimal)priceInRon);
}
