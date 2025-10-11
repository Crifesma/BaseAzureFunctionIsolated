using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ProyectoUI.Controllers
{
    public class SincronizeFunction
    {
        private readonly ILogger<SincronizeFunction> _logger;

        public SincronizeFunction(
            ILogger<SincronizeFunction> logger
        )
        {
            _logger = logger;
        }

        [Function("Sincronize")]
        [FixedDelayRetry(5, "00:00:10")]
        public async Task Run(
            [TimerTrigger("0 0 * * * *", RunOnStartup = true)] TimerInfo timerInfo, FunctionContext context)
        {
            _logger.LogInformation($"La función Sincronize se ejecutó a las: {DateTime.Now}");

            try
            {
                //dev
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en la sincronización: {ex.Message}");
                throw;
            }
        }
    }
}
