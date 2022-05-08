using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdotcode.DataLib.Core;

public class TableNameAttribute : Attribute
{
    public TableNameAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
