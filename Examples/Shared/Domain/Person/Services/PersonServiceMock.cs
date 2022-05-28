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
            new PersonModel { Id = 4, Name = "Bill" },
            new PersonModel { Id = 5, Name = "Ted" },
            new PersonModel { Id = 6, Name = "Homer" },
            new PersonModel { Id = 7, Name = "Lisa" },
            new PersonModel { Id = 8, Name = "Marge" },
            new PersonModel { Id = 9, Name = "Maggie" },
        });
    }
}
