using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using sdotcode.DataLib.Core;
using sdotcode.DataLib.Core.DataStores;
using sdotcode.DataLib.Example.BlazorApp;
using sdotcode.DataLib.Examples;
using sdotcode.DataLib.Examples.Entities;
using System.Reflection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7051/") });

//builder.Services.AddWebApiRepositoryFor<PersonModel>();

builder.Services.AddWebApiRepositoryFor("sdotcode.DataLib.Examples.Entities");

//builder.Services.AddWebApiRepositoryFor("sdotcode.DataLib.Examples.Entities", new Dictionary<Type, Type>
//{
//    [typeof(PersonModel)] = typeof(PersonServiceMock)
//});

await builder.Build().RunAsync();
