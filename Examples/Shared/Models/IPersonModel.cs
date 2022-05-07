using sdotcode.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[TableName("People")]
public class IPersonModel : IStoredItem
{
    public virtual int Id { get; set; }
    public virtual string? Name { get; set; } = string.Empty;
}