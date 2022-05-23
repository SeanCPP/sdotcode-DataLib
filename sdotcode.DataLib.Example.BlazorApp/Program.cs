using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using sdotcode.DataLib.Core;
using sdotcode.DataLib.Example.BlazorApp;
using sdotcode.DataLib.Examples;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7051/") });

builder.Services.AddHttpClientDataStore<PersonModel, PersonService>();

await builder.Build().RunAsync();
