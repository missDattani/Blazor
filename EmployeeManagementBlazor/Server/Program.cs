using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ServerLibrary.Helpers;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Repositories.Implementations;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BaseLibrary.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string credentialPath = @"C:\Users\SIT307 .SIT\Desktop\Lerning\Blazor\employeedata-82b57-firebase-adminsdk-vsyiq-8ee092005a.json";
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
var jwtSection = builder.Configuration.GetSection(nameof(JwtConfig)).Get<JwtConfig>();
builder.Services.AddScoped<IUserAccount, UserAccountRepository>();

builder.Services.AddScoped<IGenericRepositoryInterface<GeneralDepartment>, GeneralDepartmentRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<Department>, DepartmentRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<Branch>, BranchRepository>();

builder.Services.AddScoped<IGenericRepositoryInterface<Country>, CountryRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<City>, CityRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<Town>, TownRepository>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = jwtSection!.Issuer,
        ValidAudience = jwtSection.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection.Key!))
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWasm", 
        builder => builder
        .WithOrigins("http://localhost:5031")
        .AllowAnyMethod() 
        .AllowAnyHeader()
        .AllowCredentials());
});
// Access the Firebase ProjectId from the appsettings.json file
string firebaseProjectId = builder.Configuration.GetSection("FirebaseConnection:ProjectId").Value!;
// Initialize Firestore
FirestoreDb firestoreDb = FirestoreDb.Create(firebaseProjectId);
builder.Services.AddSingleton(firestoreDb); // Register FirestoreDb for dependency injection
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowBlazorWasm");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
