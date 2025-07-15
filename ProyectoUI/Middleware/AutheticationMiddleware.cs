using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
//using Microsoft.Identity.Web;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace AzureFunctionPB.Middleware
{
    public class AutheticationMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            bool authenticationStatus = false;
            HttpRequestData? httpRequestData = await context.GetHttpRequestDataAsync();

            try
            {
                if (httpRequestData != null)
                {
                    if (
                        context.Items.TryGetValue(
                            "HttpRequestContext",
                            out object httpRequestContextObj
                        )
                    )
                    {
                        DefaultHttpContext httpRequestContext =
                            (DefaultHttpContext)httpRequestContextObj;
                        //var (Status, authenticationResponse) =
                        //    await httpRequestContext.AuthenticateAzureFunctionAsync();q
                        //context.Items.Add("email", httpRequestContext.User.Identity.Name);
                        //authenticationStatus = Status;
                    }
                }
            }
            catch (Exception ex) { }

            if (!authenticationStatus)
            {
                httpRequestData.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }

            await next(context);
        }
    }
}
