# sdotcode-DataLib
A database-agnostic Symmetrical Repository Pattern implementation that allows you to create DI-friendly CRUD respositories/services for model items

## The Symmetrical Repository pattern

When the entire tech stack of a product is written in one unified **.net**, we can start layering some traditional design patterns outwards, across parts of the stack.
Reusing .net code doesn't need to stop at sharing models in a shared project. Since *Dependency Injection* is so first-class in .net, the repository pattern has
proven its worth in production full-stack .net applications, and lends to being a fantastic developer experience.

In fact, over the past few production .net full-stack applications have really honed in this new pattern that was looking me straight in the eye. Since certain tasks in 
regard to data acess have become so trivialized by the abstractions brought into the .net ecosystem that we can start *DRY*ing our code across the tech stack somewhat 
seamlessly.

The repository pattern can be mirrored between the back-end and the front-end, which provides you a seamless approach to dealing with boring CRUD operations across layers
of the tech stack.

## Model class
```csharp
[TableName("People")]
public class IPersonModel : IStoredItem
{
    public virtual int Id { get; set; }
    public virtual string? Name { get; set; } = string.Empty;
}
```

## Service class
```csharp
public class PersonService : Service<IPersonModel>
{
    public PersonService(IDataStore<IPersonModel> dataStore) : base(dataStore)
    {
    }
}
```

That's all you need for a basic CRUD service abstraction you can **Inject** into your API controller, Blazor component, etc

In order to make it work, you need to plug an IDataStore implemenation into your **DI** container. So far, there are two (2) built-in IDataStore implementations:
**InMemoryDataStore** and **HttpClientDataStore**

The first one is essentially just an adapter for a List<T> which allows for easy mocking abstractions
   
## The Mock class
```csharp
public PersonServiceMock() 
    : base(new InMemoryDataStore<IPersonModel>())
{
    AddOrUpdateAsync(new List<IPersonModel>()
    {
        new PersonModel { Id = 1, Name = "Curly" },
        new PersonModel { Id = 2, Name = "Larry" },
        new PersonModel { Id = 3, Name = "Moe" },
    });
```

 The second IDataStore is more interesting. 
 ## The HttpClientDataStore
 
 To make this pattern truly symmetrical when doing HTTP requests to an API, we ensure our controllers follow a REST convention.
 
 ### The PeopleController class
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
    
By subclassing the ExtendedControllerBase, we get a fully-featured CRUD API for that model class.
    
When HttpClientDataStore is registered as the IDataStore in your front-end app (Blazor, winforms, console app, etc)
It will make HTTP requests to the appropriate endpoints to handle the data.
    
### The Blazor project's Program.cs
```csharp
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7051/") });

builder.Services.AddScoped<IDataStore<IPersonModel>, HttpClientDataStore<IPersonModel>>();
builder.Services.AddScoped<Service<IPersonModel>, PersonService>();
```
    
 It's important to note that the HttpClient must be properly configured to point to the API project, and your API project must enable CORS from your front-end applcation.

### The API Project's Program.cs
```csharp
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:7220");
        });
});

var app = builder.Build();

// Later on...
app.UseCors(MyAllowSpecificOrigins);
```
    
 And under the hood, the HttpClientDataStore will send requests to the API, which is using its own symmetrical Service repository.
    
 
Getting data is as simple as:
    
### Blazor component
```csharp
@foreach(var person in people)
{
    <p>@person.Id - @person.Name</p>
}

@code {
    [Inject] Service<IPersonModel>? service { get; set; }

   

    IEnumerable<IPersonModel> people = new List<IPersonModel>();

    protected override async Task OnInitializedAsync()
    {
        people = await service!.GetAsync();
    }
}
```
