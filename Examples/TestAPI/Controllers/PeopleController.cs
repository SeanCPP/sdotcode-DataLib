using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sdotcode.DataLib.Examples;
using sdotcode.Repository;

namespace TestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ExtendedControllerBase<IPersonModel>
    {
        public PeopleController(Service<IPersonModel> service) : base(service)
        {
        }
    }
}
