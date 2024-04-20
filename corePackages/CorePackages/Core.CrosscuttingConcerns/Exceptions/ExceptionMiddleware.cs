using Core.CrosscuttingConcerns.Exceptions.Handlers;
using Core.CrosscuttingConcerns.Logging;
using Core.CrosscuttingConcerns.Serilog;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Core.CrosscuttingConcerns.Exceptions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpExceptionHandler _httpExceptionHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoggerServiceBase _loggerService;

        public ExceptionMiddleware(RequestDelegate next,IHttpContextAccessor httpContextAccessor,LoggerServiceBase loggerService)
        {
            _next = next;
            _httpExceptionHandler = new HttpExceptionHandler();
            _httpContextAccessor = httpContextAccessor;
            _loggerService = loggerService;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context.Response, ex);
                await LogExecption(context, ex);
            }
        }

        private Task LogExecption(HttpContext context, Exception ex)
        {
            List<LogParameter> parameters = new()
            {
                new LogParameter{ Type = context.GetType().Name,Value=ex.ToString()}
            };

            LogDetailWithException logDetailWithException = new()
            {
                ExceptionMessage = ex.Message,
                LogParameters = parameters,
                MethodName = _next.Method.Name,
                User = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "?"
            };

            _loggerService.Error(JsonSerializer.Serialize(logDetailWithException));

            return Task.CompletedTask;
        }

        private Task HandleExceptionAsync(HttpResponse response ,Exception exception)
        {
            response.ContentType = "application/json";
            _httpExceptionHandler.Response = response;

            return _httpExceptionHandler.HandleExceptionAsync(exception);

        }
    }
}
