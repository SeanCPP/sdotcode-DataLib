# sdotcode-DataLib
A database-agnostic Symmetrical Repository Pattern implementation that allows you to create DI-friendly CRUD respositories/services for model items

## The Symmetrical Repository pattern

  Since certain tasks (in regard to data acess) have become so trivialized by the abstractions brought into the .net ecosystem, we can start *DRY*ing our code across the tech stack pretty seamlessly. The repository pattern can be mirrored between the back-end and the front-end, which provides you a seamless approach to dealing with boring CRUD operations across layers of the tech stack.
  
![ex1](https://user-images.githubusercontent.com/4634215/167278164-cf47c839-4cf8-44a2-be20-87fbea3cee7a.png)

# Using this as your **_solution-wide_** repository layer

### Model class
```csharp
[TableName("People")]
public class IPersonModel : IStoredItem
{
    public virtual int Id { get; set; }
    
    [Searchable]
    public virtual string? Name { get; set; } = string.Empty;
}
```

### Service class
```csharp
public class PersonService : Service<IPersonModel>
{
    public PersonService(IDataStore<IPersonModel> dataStore) : base(dataStore)
    {
    }
}
```

 ### The PeopleController class (if using the API features)
 ```csharp
[ApiController]
[Route("[controller]")]
public class PeopleController : ExtendedControllerBase<IPersonModel>
{
    public PeopleController(Service<IPersonModel> service) : base(service)
    {
    }
}
```

With these classes set up and registered in the DI container, you could now utilize Service<PersonModel> in any project in the solution and perform CRUD and Search operations in an API controller, Blazor component, Xamarin/MAUI app, etc.

### Blazor component
```csharp
@foreach(var person in people)
{
    <p>@person.Id - @person.Name</p>
}

<input type="text" @bind-value=nameSearch />
<button @onclick=Search>Search</button>

@code {
    [Inject] Service<PersonModel>? service { get; set; }

    private string nameSearch = string.Empty;

    IEnumerable<PersonModel> people = new List<PersonModel>();

    private async Task Search()
    {
        people = await service!.SearchAsync(nameSearch, 
            pagingOptions: default,
            x => x.Name, 
            x => x.Id); // This won't get searched since the Id property on PersonModel doesn't have [Searchable]
    }
}
```

## The IDataStore interface
In order to make this work, we need to plug an IDataStore implemenation into the DI containers of each application. So far, there are two (2) built-in IDataStore implementations (more are coming):
**InMemoryDataStore** and **HttpClientDataStore**

**Note** If you need an IDataStore for your database/data source, simply implement the IDataStore<T> interface.
  
The **InMemoryDataStore** is an in-memory data store that can be used for mocking / testing.

 The **HttpClientDataStore** is more interesting. 
 
### The HttpClientDataStore
    
By subclassing the ExtendedControllerBase in your code, you get a fully-featured CRUD+Search API for that model class generated automatically.
    
When using the Service class with HttpClientDataStore registered as the IDataStore, the Service will make HTTP requests to the appropriate controller methods behind the scenes. 

If you need to add role authorization to any controller method, you can always override the method in your controller and add the [Authorize] attribute:
```csharp
[Authorize]
public override Task<ActionResult> Upsert([FromBody] IEnumerable<PersonModel> items) => base.Upsert(items);
```

## Wiring up the DI
  
It's important to note that in order for the system to automatically wire up and communicate with the API, the HttpClient must be properly configured to point to the API project, and your API project must allow CORS from your front-end applcation.
  
### The Blazor project's Program.cs
```csharp
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7051/") });

builder.Services.AddScoped<IDataStore<IPersonModel>, HttpClientDataStore<IPersonModel>>();
builder.Services.AddScoped<Service<IPersonModel>, PersonService>();
```

### The API Project's Program.cs
```csharp
builder.Services.AddSingleton<IDataStore<IPersonModel>, InMemoryDataStore<IPersonModel>>();
builder.Services.AddSingleton<Service<IPersonModel>, PersonServiceMock>();
  
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:7220"); // Front-end application that makes requests to this API
        });
});

var app = builder.Build();

// Later on...
app.UseCors(MyAllowSpecificOrigins);
```
    
 And under the hood, the HttpClientDataStore will send requests to the API, which is using its own symmetrical Service repository.
