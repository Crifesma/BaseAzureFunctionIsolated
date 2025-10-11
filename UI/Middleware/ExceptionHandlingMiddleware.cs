using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System.Net;

public class ExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en la función");

            var httpRequestData = await context.GetHttpRequestDataAsync();
            if (httpRequestData != null)
            {
                var response = httpRequestData.CreateResponse();
                await response.WriteAsJsonAsync(new { error = "Error interno del servidor" });
                response.StatusCode = HttpStatusCode.InternalServerError;

                context.GetInvocationResult().Value = response;
            }
        }
    }
}
