namespace BlazorAppEcommerce.Services;

public class CountriesService
{
    public Task<List<string>> GetCountriesAsync()
    {
        var countries = new List<string>
        {
            "Afghanistan", "Albania", "Algeria", "Andorra", "Angola",
            "Argentina", "Armenia", "Australia", "Austria", "Azerbaijan",
            "Bahrain", "Bangladesh", "Belarus", "Belgium", "Bolivia",
            "Brazil", "Bulgaria", "Cambodia", "Canada", "Chile",
            "China", "Colombia", "Croatia", "Cuba", "Cyprus",
            "Czech Republic", "Denmark", "Ecuador", "Egypt", "Estonia",
            "Ethiopia", "Finland", "France", "Georgia", "Germany",
            "Ghana", "Greece", "Hungary", "India", "Indonesia",
            "Iran", "Iraq", "Ireland", "Israel", "Italy",
            "Japan", "Jordan", "Kazakhstan", "Kenya", "Kuwait",
            "Lebanon", "Libya", "Lithuania", "Malaysia", "Mexico",
            "Morocco", "Netherlands", "New Zealand", "Nigeria", "Norway",
            "Oman", "Pakistan", "Palestine", "Peru", "Philippines",
            "Poland", "Portugal", "Qatar", "Romania", "Russia",
            "Saudi Arabia", "Serbia", "Singapore", "Somalia", "South Africa",
            "South Korea", "Spain", "Sudan", "Sweden", "Switzerland",
            "Syria", "Thailand", "Tunisia", "Turkey", "UAE",
            "Ukraine", "United Kingdom", "United States", "Venezuela", "Yemen"
        };

        return Task.FromResult(countries.OrderBy(c => c).ToList());
    }
}