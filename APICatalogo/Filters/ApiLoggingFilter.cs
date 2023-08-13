using Microsoft.AspNetCore.Mvc.Filters;

namespace APICatalogo.Filters
{
    public class ApiLoggingFilter : IActionFilter
    {
        private readonly ILogger<ApiLoggingFilter> _logger;

        public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation($"Executando -> {nameof(OnActionExecuting)}");
            _logger.LogInformation("{Date}", DateTime.Now.ToLongTimeString());
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation($"Executando -> {nameof(OnActionExecuted)}");
            _logger.LogInformation("{Date}", DateTime.Now.ToLongTimeString());
        }
    }
}
