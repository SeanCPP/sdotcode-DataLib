using sdotcode.DataLib.Core;
using sdotcode.DataLib.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdotcode.DataLib.Example
{
    [TableName("Posts")]
    public class PostModel : IStoredItem
    {
        public virtual int Id { get; set; }
        public virtual string? Content { get; set; }
        public virtual PersonModel? Author { get; set; }
        public virtual DateTime Created { get; set; }
    }
}
