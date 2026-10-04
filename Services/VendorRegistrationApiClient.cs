using System.Net.Http.Json;

namespace FoodServiceApp.Maui.Services;

public class VendorRegistrationDto
{
    public string Id { get; set; } = "";
    public string OwnerName { get; set; } = "";
    public string StoreName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
    public string BusinessType { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int Status { get; set; }
}

public class VendorRegistrationApiClient
{
    private readonly HttpClient _httpClient;

    public VendorRegistrationApiClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5234/")
        };
    }

    public async Task<List<VendorRegistrationDto>> GetPendingAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<VendorRegistrationDto>>(
                   "api/vendor-registrations/pending")
               ?? new List<VendorRegistrationDto>();
    }

    public async Task<List<VendorRegistrationDto>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<VendorRegistrationDto>>(
               "api/vendor-registrations")
           ?? new List<VendorRegistrationDto>();
    }

    public async Task<bool> ApproveAsync(string id)
    {
        var response = await _httpClient.PostAsync(
            $"api/vendor-registrations/{id}/approve",
            null);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RejectAsync(string id)
    {
        var response = await _httpClient.PostAsync(
            $"api/vendor-registrations/{id}/reject",
            null);

        return response.IsSuccessStatusCode;
    }
}