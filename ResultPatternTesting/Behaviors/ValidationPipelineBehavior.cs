using FluentValidation;
using MediatR;
using System.Windows.Input;

namespace ResultPatternTesting.Behaviors
{
    public class ValidationPipelineBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger _logger;

        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationPipelineBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if(!_validators.Any()) { return await next(); }
            Error[] errors = _validators
                .Select(validator => validator.Validate(request))
                .SelectMany(validationResult => validationResult.Errors)
                .Where(validationFailure => validationFailure is not null)
                .Select(failure => new Error(failure.PropertyName, failure.ErrorMessage))
                .Distinct()
                .ToArray();
            if (errors.Any())
            {
                //_logger.LogError("");
                return CreateValidationResult<TResponse>(errors);
            }
            return await next();
        }
        private static TResult CreateValidationResult<TResult>(Error[] errors)
            where TResult : Result
        {
            if(typeof(TResult) == typeof(Result))
            {
                //this will never failt so we are sure its not null.
                return (ValidationResult.WithErrors(errors) as TResult)!;
            }

            //this will never fail so we are sure its not null.
            object validationResult = typeof(ValidationResult<>)
                .GetGenericTypeDefinition()
                //wyjaśnienie https://learn.microsoft.com/pl-pl/dotnet/api/system.type.makegenerictype?view=net-8.0
                .MakeGenericType(typeof(TResult).GenericTypeArguments[0])
                .GetMethod(nameof(ValidationResult.WithErrors))!
                .Invoke(null, new object?[] { errors })!;
            //object validationResult = ValidationResult<TResult>.WithErrors(errors);
            return (TResult)validationResult;
        }
    }
}
