﻿using sdotcode.DataLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdotcode.DataLib.Examples;

[TableName("People")]
public class IPersonModel : IStoredItem
{
    public virtual int Id { get; set; }
    public virtual string? Name { get; set; } = string.Empty;
}