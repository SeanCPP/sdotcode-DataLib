using sdotcode.DataLib.Core;
using sdotcode.DataLib.Core.DataStores;

namespace sdotcode.DataLib.Examples;

public class PersonServiceMock : Service<PersonModel>
{
    public PersonServiceMock() 
        : base(new InMemoryDataStore<PersonModel>())
    {
        AddOrUpdateAsync(new List<PersonModel>()
        {
            new PersonModel { Id = 1, Name = "Curly" },
            new PersonModel { Id = 2, Name = "Larry" },
            new PersonModel { Id = 3, Name = "Moe" },
        });
    }
}
