using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SimpleBlog.Infrastructures.Exceptions;

namespace SimpleBlog.Infrastructures.Filters
{
    public class StatusCodeExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<StatusCodeExceptionFilter> _logger;

        public StatusCodeExceptionFilter(ILogger<StatusCodeExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (!(context.Exception is StatusCodeException exception))
                return;

            context.Result = new StatusCodeResult(exception.StatusCode);

            _logger.LogWarning(exception,
                $"An StatusCodeException {exception.StatusCode} exception has occurred. {exception.Message}");
        }
    }
}
