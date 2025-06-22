using FluentValidation;

namespace FileService.Api
{
    public class ValidationHelper
    {
        public static async Task ValidateModelAsync<T>(T model, IValidator<T> validator)
        {
            var result = await validator.ValidateAsync(model);
            if (!result.IsValid)
            {
                var message = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(message);
            }
        }
    }
}
