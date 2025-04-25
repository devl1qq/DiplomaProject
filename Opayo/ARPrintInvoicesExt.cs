using Microsoft.AspNetCore.Http;
using Opayo.Tools;
using PX.Api.Webhooks.Graph;
using PX.Data;
using PX.Data.DependencyInjection;
using PX.Objects.AR;
using PX.Objects.AR.MigrationMode;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Opayo
{ 
    public class ARPrintInvoicesExt : HttpContextInitializator<ARPrintInvoices> { }
}
    
    
