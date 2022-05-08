using Microsoft.AspNetCore.Mvc;
using sdotcode.DataLib.Core;
using sdotcode.DataLib.Core.Api;
using sdotcode.DataLib.Examples;

namespace sdotcode.DataLib.Example.TestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ExtendedReadOnlyControllerBase<PostModel>
    {
        public PostsController(ReadOnlyService<PostModel> service) : base(service)
        {
        }
    }
}
