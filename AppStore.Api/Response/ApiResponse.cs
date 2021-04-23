using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStore.Api.Response
{
    public class ApiResponse<T>
    {
        public ApiResponse(T data)
        {
            Body = data;
        }
        public T Body { get; set; }
    }
}
