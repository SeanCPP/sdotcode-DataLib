using sdotcode.DataLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdotcode.DataLib.Example
{
    public class PostService : Service<PostModel>
    {
        public PostService(IDataStore<PostModel> dataStore) : base(dataStore)
        {
        }
    }
}
