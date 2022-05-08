using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sdotcode.DataLib.Core;
using sdotcode.DataLib.Core.Api;
using sdotcode.DataLib.Examples;

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
