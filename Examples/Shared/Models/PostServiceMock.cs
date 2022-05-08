using sdotcode.DataLib.Core;
using sdotcode.DataLib.Core.DataStores;
using sdotcode.DataLib.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdotcode.DataLib.Example
{
    public class PostServiceMock : Service<PostModel>
    {
        public PostServiceMock()
            : base(new InMemoryDataStore<PostModel>())
        {
            DataStore.AddOrUpdateAsync(new List<PostModel>()
            {
                new PostModel { Id = 1, Content = "Curly", Author = new PersonModel { Id = 1, Name = "Curly" } },
                new PostModel { Id = 2, Content = "Larry", Author = new PersonModel { Id = 1, Name = "Curly" } },
                new PostModel { Id = 3, Content = "Moe", Author = new PersonModel { Id = 1, Name = "Curly" } },
            });
        }
    }
}
