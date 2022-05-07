using sdotcode.Repository;
using sdotcode.Repository.DataStores;

public class PersonServiceMock : Service<IPersonModel>
{
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
}
