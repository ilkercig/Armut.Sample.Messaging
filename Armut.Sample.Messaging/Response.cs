using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Armut.Sample.Messaging
{
    public class Response<T> : ResponseBase
    {
        public T Result { get; set; }
    }
}
