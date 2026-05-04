using Clinica.WASM;
using Clinica.WASM.Services.Api;
using Clinica.WASM.Services.Auth;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddScoped<TokenStorageService>();
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<AuthHeaderHandler>();
builder.Services.AddScoped<ApiErrorService>();
//funcionalidades
builder.Services.AddScoped<PacienteApiService>();

builder.Services.AddHttpClient("ClinicaApi", client =>
    {
        client.BaseAddress = new Uri("https://localhost:7241/"); // NOSONAR
    })
    .AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ClinicaApi"));

builder.Services.AddScoped<AuthApiService>();

await builder.Build().RunAsync();