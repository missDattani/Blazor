using BaseLibrary.Entities;
using Blazored.LocalStorage;
using Client;
using Client.ApplicationState;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;
using ClientLibrary.Services.Implementations;
using Google.Api;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<CustomHttpHandler>();
builder.Services.AddHttpClient("SystemApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5037/");
}).AddHttpMessageHandler<CustomHttpHandler>();
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5037/") });
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<GetHttpClient>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IUserAccountService,UserAccountService>();

builder.Services.AddScoped<IGenericInterfaceService<GeneralDepartment>, GenericServiceImplementation<GeneralDepartment>>();
builder.Services.AddScoped<IGenericInterfaceService<Department>, GenericServiceImplementation<Department>>();
builder.Services.AddScoped<IGenericInterfaceService<Branch>, GenericServiceImplementation<Branch>>();

builder.Services.AddScoped<IGenericInterfaceService<Country>, GenericServiceImplementation<Country>>();
builder.Services.AddScoped<IGenericInterfaceService<City>, GenericServiceImplementation<City>>();
builder.Services.AddScoped<IGenericInterfaceService<Town>, GenericServiceImplementation<Town>>();

builder.Services.AddScoped<IGenericInterfaceService<Employee>, GenericServiceImplementation<Employee>>();

builder.Services.AddScoped<IToastrService,ToastrService>();
builder.Services.AddScoped<AllState>();

await builder.Build().RunAsync();
