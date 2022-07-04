using sdotcode.DataLib.Core;
using sdotcode.DataLib.Core.DataStores;
using sdotcode.DataLib.Examples.Entities;

namespace sdotcode.DataLib.Examples;

public class PersonServiceMock : Service<PersonModel>
{
    public PersonServiceMock() 
        : base(new InMemoryDataStore<PersonModel>())
    {
        UpsertAsync(new List<PersonModel>()
        {
            new PersonModel { Id = 1, Name = "Curly", Email = "TestMail1@email.com" },
            new PersonModel { Id = 2, Name = "Larry", Email = "Mail2Test@email.com" },
            new PersonModel { Id = 3, Name = "Moe", Email = "TEmail3@email.com" },
            new PersonModel { Id = 4, Name = "Bill", Email = "Email4Tests@email.com" },
            new PersonModel { Id = 5, Name = "Ted", Email = "Test5Emails@email.com" },
            new PersonModel { Id = 6, Name = "Homer", Email = "Test@email.com" },
            new PersonModel { Id = 7, Name = "Lisa", Email = "Email@test.com" },
            new PersonModel { Id = 8, Name = "Marge", Email = "Email@email.com" },
            new PersonModel { Id = 9, Name = "Maggie", Email = "TestEmailEmail@email.com" },
        });
    }
}
