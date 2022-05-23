using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sdotcode.DataLib.Core;

namespace sdotcode.DataLib.Core.Api
{
    public abstract class ExtendedControllerBase<T> : ControllerBase
        where T : IStoredItem, new()
    {
        protected readonly Service<T> Service;
        public ExtendedControllerBase(Service<T> service)
        {
            Service = service;
        }

        [HttpGet]
        [Route("Find/{propertyName}/{value}")]
        public virtual async Task<ActionResult> Get([FromRoute] string propertyName,
            [FromRoute] string value,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = Defaults.PageSize)
        {
            return Ok(await Service.GetAsync(propertyName, value, new PagingInfo { Page = page, PageSize = pageSize }));
        }

        [HttpGet]
        public virtual async Task<ActionResult> Get(int page = 0, int pageSize = 10)
        {
            return Ok(await Service.GetAsync(new PagingInfo { Page = page, PageSize = pageSize }));
        }

        [HttpGet]
        [Route("Search")]
        public virtual async Task<ActionResult> Search([FromQuery] string[] searchProperties,
            [FromQuery] string query,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = Defaults.PageSize)
        {
            return Ok(await Service.SearchAsync(
                query,
                new PagingInfo { Page = page, PageSize = pageSize },
                searchProperties));
        }

        [HttpDelete]
        public virtual async Task<ActionResult> Delete(int id)
        {
            var result = await Service.DeleteAsync(id);
            if (result)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public virtual async Task<ActionResult> Upsert([FromBody] IEnumerable<T> items)
        {
            if (items == null || !items.Any())
            {
                return BadRequest("Invalid item passed");
            }
            return Ok(await Service.AddOrUpdateAsync(items));
        }
    }
}
