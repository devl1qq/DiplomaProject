using Microsoft.AspNetCore.Http;
using PX.Data.DependencyInjection;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opayo.Tools
{   
    public static class HttpAccessor
    {
        public static HttpContext HttpContext { get; set; }
    }
}
