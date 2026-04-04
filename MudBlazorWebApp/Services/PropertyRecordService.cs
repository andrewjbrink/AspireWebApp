using AspireWebApp.Domain.ValuationRoll;

namespace MudBlazorWebApp.Services;

public class PropertyRecordService
{
    private readonly HttpClient _http;

    public PropertyRecordService(HttpClient http)
    {
        _http = http;
    }

    public async Task<PropertyRecord> GetPropertyRecord()
    {
        //const endpoint = window.__config.apiBaseUrl + "/api/valuations/ErfValuation/" + erfName + "/" + minRegion.replace(" ", "%20"); //


        var propertyRecord = await _http.GetFromJsonAsync<PropertyRecord>("api/PropertyRecord");
        return propertyRecord!;

    }
}
