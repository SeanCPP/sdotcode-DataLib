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
```csharp
InMemoryDataStore<T>
```

and

```csharp
HttpClientDataStore<T>
```

The first one is essentially just an adapter for a List<T> which allows for easy mocking abstractions.
  
  Mock class:
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
    }
  ```
  
  The second one is more interesting.
 
