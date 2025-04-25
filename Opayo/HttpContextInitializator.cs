using Microsoft.AspNetCore.Http;
using Opayo.Tools;
using PX.Data.DependencyInjection;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opayo
{
    public abstract class HttpContextInitializator<TGraph> : PXGraphExtension<TGraph>, IGraphWithInitialization where TGraph : PXGraph
    {
        [InjectDependency]
        private IHttpContextAccessor _httpContextAccessor { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            try
            {
                if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request.Scheme != null)
                {
                    HttpAccessor.HttpContext = _httpContextAccessor.HttpContext;
                }
            }
            catch { }
        }
    }
}
