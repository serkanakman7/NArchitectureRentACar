using Core.CrosscuttingConcerns.Exceptions.Types;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Pipelines.Validation
{
    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validator;

        public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validator)
        {
            _validator = validator;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            ValidationContext<object> context = new(request);

            IEnumerable<ValidationExceptionModel> errors = _validator
                .Select(validator => validator.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(failure => failure != null)
                .GroupBy(
                keySelector: p => p.PropertyName,
                resultSelector: (propertyName, error) =>
                new ValidationExceptionModel()
                {
                    Property = propertyName,
                    Errors = error.Select(e => e.ErrorMessage)
                }).ToList();

            if (errors.Any())
                throw new CrosscuttingConcerns.Exceptions.Types.ValidationException(errors);

            TResponse response = await next();
            return response;
        }
    }
}
