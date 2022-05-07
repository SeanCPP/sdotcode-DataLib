using sdotcode.Repository;

public class PersonService : Service<IPersonModel>
{
    public PersonService(IDataStore<IPersonModel> dataStore) : base(dataStore)
    {
    }
}
