using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdotcode.DataLib.Core.Api
{
    public class ExtendedReadonlyControllerBase<T> : ControllerBase 
        where T : IStoredItem, new()
    {
        protected readonly ReadOnlyService<T> Service;
        public ExtendedReadonlyControllerBase(ReadOnlyService<T> service)
        {
            this.Service = service;
        }

        [HttpGet]
        [Route("find/{propertyName}/{value}")]
        public virtual async Task<ActionResult> Get([FromRoute] string propertyName, [FromRoute] string value)
        {
            return Ok(await Service.GetAsync(propertyName, value));
        }

        [HttpGet]
        public virtual async Task<ActionResult> Get(int page = 0, int pageSize = 10)
        {
            return Ok(await Service.GetAsync(page, pageSize));
        }
    }
}
