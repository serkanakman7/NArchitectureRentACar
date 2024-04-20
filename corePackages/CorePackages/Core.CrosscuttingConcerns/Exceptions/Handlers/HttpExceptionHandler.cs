using Core.CrosscuttingConcerns.Exceptions.Extensions;
using Core.CrosscuttingConcerns.Exceptions.HttpProblemDetails;
using Core.CrosscuttingConcerns.Exceptions.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidationProblemDetails = Core.CrosscuttingConcerns.Exceptions.HttpProblemDetails.ValidationProblemDetails;

namespace Core.CrosscuttingConcerns.Exceptions.Handlers
{
    public class HttpExceptionHandler : ExceptionHandler
    {
        private HttpResponse? _response;

        public HttpResponse Response
        {
            get=> _response ?? throw new ArgumentNullException(nameof(_response));
            set => _response = value;
        }
        protected override Task HandleException(BusinessException businessException)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            string detail = new BusinessProblemDetails(businessException.Message).AsJson();
            return Response.WriteAsync(detail);
        }

        protected override Task HandleException(Exception exception)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            string detail = new InternalServerErrorProblemDetails().AsJson();
            return Response.WriteAsync(detail);
        }

        protected override Task HandleException(ValidationException validationException)
        {
            Response.StatusCode =StatusCodes.Status400BadRequest;
            string detail = new ValidationProblemDetails(validationException.Errors).AsJson();
            return Response.WriteAsync(detail);
        }
    }
}
