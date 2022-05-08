using sdotcode.DataLib.Core;

namespace sdotcode.DataLib.Examples;
/// <summary>
/// This is all you have to write to get a fully featured CRUD service for a model type that you can inject 
/// anywhere in the tech stack
/// </summary>
public class PersonService : Service<PersonModel>
{
    public PersonService(IDataStore<PersonModel> dataStore) : base(dataStore)
    {
    }
}
