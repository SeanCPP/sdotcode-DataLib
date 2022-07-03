using sdotcode.DataLib.Core;
using sdotcode.DataLib.Core.DataStores;
using sdotcode.DataLib.Examples;

var service = Factory.GetPersonService();

Console.ReadKey();
var people = await service.GetAsync();

foreach(var person in people)
{
    Console.WriteLine(person.Name);
}

Console.ReadKey();

var add = await service.UpsertAsync(new PersonModel {  Id=1, Name = "Gary?"});
Console.WriteLine(add.Name);

Console.ReadKey();

bool result = await service.DeleteAsync(1);
if (result)
{
    Console.WriteLine("Deleted successfully.");
}


public static class Factory
{
    public static Service<PersonModel> GetPersonService()
    {
        return new Service<PersonModel>(GetDataStore<PersonModel>());
    }

    public static IDataStore<T> GetDataStore<T>()
        where T : IStoredItem, new()
    {
        return new HttpClientDataStore<T>(new HttpClient { BaseAddress = new Uri("https://localhost:7051/") });
    }
}