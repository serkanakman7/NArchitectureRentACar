﻿using Core.CrosscuttingConcerns.Exceptions.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.CrosscuttingConcerns.Exceptions.HttpProblemDetails
{
    public class ValidationProblemDetails : ProblemDetails
    {
        public IEnumerable<ValidationExceptionModel> Errors { get; init; }
        public ValidationProblemDetails(IEnumerable<ValidationExceptionModel> errors)
        {
            Title = "Validation error(s)";
            Detail = "One or more  validation errors accurred";
            Errors = errors;
            Status = StatusCodes.Status400BadRequest;
            Type = "https://example.com/probs/validation";

        }
    }
}
