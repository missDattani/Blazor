﻿using BaseLibrary.DTOs;


namespace ClientLibrary.Helpers
{
    public class GetHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly LocalStorageService _storageService;

        public GetHttpClient(IHttpClientFactory httpClientFactory, LocalStorageService storageService)
        {
            _httpClientFactory = httpClientFactory;
            _storageService = storageService;
        }

        private const string HeaderKey = "Authorization";
        public async Task<HttpClient> GetPrivateHttpClient()
        {
            var client = _httpClientFactory.CreateClient("SystemApiClient");
            var stringToken = await _storageService.GetToken();
            if(string.IsNullOrEmpty(stringToken)) return client;

            var deserializeToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
            if(deserializeToken == null) return client;

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", deserializeToken.Token);
            return client;
            
        }

        public HttpClient GetPublicHttpClient()
        {
            var client = _httpClientFactory.CreateClient("SystemApiClient");
            client.DefaultRequestHeaders.Remove(HeaderKey);

            return client;
        }
    }
}
