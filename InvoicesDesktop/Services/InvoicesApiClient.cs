using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using InvoicesDesktop.Models;
using InvoicesDesktop.Models.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace InvoicesDesktop.Services;

/// <summary>
/// Typed HTTP client for the Invoices API. The API is hosted on localhost over
/// http on the port declared in the project's launch settings (5239).
/// </summary>
public class InvoicesApiClient {

    public const string BaseUrl = "http://localhost:5239/";

    private readonly HttpClient _http;
    private readonly JsonSerializerSettings _jsonSettings;

    public InvoicesApiClient() {
        _http = new HttpClient {
            BaseAddress = new Uri(BaseUrl)
        };

        _jsonSettings = new JsonSerializerSettings {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    // ── Invoices ─────────────────────────────────────────────────────────────
    public Task<List<InvoiceModel>> GetInvoicesAsync()
        => GetAsync<List<InvoiceModel>>("api/v1/invoices");

    public Task<InvoiceModel?> CreateInvoiceAsync(InvoiceRequest request)
        => PostAsync<InvoiceModel>("api/v1/invoices", request);

    public Task<InvoiceModel?> UpdateInvoiceAsync(Guid id, InvoiceRequest request)
        => PutAsync<InvoiceModel>($"api/v1/invoices/{id}", request);

    public Task DeleteInvoiceAsync(Guid id)
        => DeleteAsync($"api/v1/invoices/{id}");

    // ── Contractors ──────────────────────────────────────────────────────────
    public Task<List<ContractorModel>> GetContractorsAsync()
        => GetAsync<List<ContractorModel>>("api/v1/contractors");

    public Task<ContractorModel?> CreateContractorAsync(ContractorRequest request)
        => PostAsync<ContractorModel>("api/v1/contractors", request);

    public Task<ContractorModel?> UpdateContractorAsync(Guid id, ContractorRequest request)
        => PutAsync<ContractorModel>($"api/v1/contractors/{id}", request);

    public Task DeleteContractorAsync(Guid id)
        => DeleteAsync($"api/v1/contractors/{id}");

    // ── Items ────────────────────────────────────────────────────────────────
    public Task<List<ItemModel>> GetItemsAsync()
        => GetAsync<List<ItemModel>>("api/v1/items");

    public Task<ItemModel?> CreateItemAsync(ItemRequest request)
        => PostAsync<ItemModel>("api/v1/items", request);

    public Task<ItemModel?> UpdateItemAsync(Guid id, ItemRequest request)
        => PutAsync<ItemModel>($"api/v1/items/{id}", request);

    public Task DeleteItemAsync(Guid id)
        => DeleteAsync($"api/v1/items/{id}");

    // ── Helpers ──────────────────────────────────────────────────────────────
    private async Task<T> GetAsync<T>(string uri) where T : new() {
        var response = await _http.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(json, _jsonSettings) ?? new T();
    }

    private async Task<T?> PostAsync<T>(string uri, object body) {
        var response = await _http.PostAsync(uri, Serialize(body));
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<T>(response);
    }

    private async Task<T?> PutAsync<T>(string uri, object body) {
        var response = await _http.PutAsync(uri, Serialize(body));
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<T>(response);
    }

    private async Task DeleteAsync(string uri) {
        var response = await _http.DeleteAsync(uri);
        response.EnsureSuccessStatusCode();
    }

    private StringContent Serialize(object body)
        => new(JsonConvert.SerializeObject(body, _jsonSettings), Encoding.UTF8, "application/json");

    private async Task<T?> DeserializeAsync<T>(HttpResponseMessage response) {
        var json = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(json)) {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
    }
}
