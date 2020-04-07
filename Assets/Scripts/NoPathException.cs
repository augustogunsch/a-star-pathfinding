using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[Serializable]
public class NoPathException : Exception
{
    public NoPathException() { }
    public NoPathException(string message) : base(message) { }
    public NoPathException(string message, Exception inner) : base(message, inner) { }
    protected NoPathException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
