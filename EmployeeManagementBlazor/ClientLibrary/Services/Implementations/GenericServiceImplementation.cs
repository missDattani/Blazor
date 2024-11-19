using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;
using System.Net.Http.Json;

namespace ClientLibrary.Services.Implementations
{
    public class GenericServiceImplementation<T> : IGenericInterfaceService<T>
    {
        private readonly GetHttpClient _getHttpClient;
        public GenericServiceImplementation(GetHttpClient getHttpClient) 
        { 
            _getHttpClient = getHttpClient;
        }

        public async Task<GeneralResponse> DeleteById(string id, string baseUrl)
        {
            var httpClient = await _getHttpClient.GetPrivateHttpClient();
            var response = await httpClient.DeleteAsync($"{baseUrl}/delete/{id}");
            var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return result!;
        }

        public async Task<List<T>> GetAll(string baseUrl)
        {
            var httpClient = await _getHttpClient.GetPrivateHttpClient();
            var result = await httpClient.GetFromJsonAsync<List<T>>($"{baseUrl}/all");
            return result!;
        }

        public async Task<T> GetById(string id, string baseUrl)
        {
            var httpClient = await _getHttpClient.GetPrivateHttpClient();
            var result = await httpClient.GetFromJsonAsync<T>($"{baseUrl}/single/{id}");
            return result!;
        }

        public async Task<GeneralResponse> Insert(T entity, string baseUrl)
        {
            var httpClient = await _getHttpClient.GetPrivateHttpClient();
            var response = await httpClient.PostAsJsonAsync($"{baseUrl}/add", entity);
            var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return result!;
        }

        public async Task<GeneralResponse> Update(T entity, string baseUrl)
        {
            var httpClient = await _getHttpClient.GetPrivateHttpClient();
            var response = await httpClient.PutAsJsonAsync($"{baseUrl}/update",entity);
            var result =  await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return result!;
        }
    }
}
