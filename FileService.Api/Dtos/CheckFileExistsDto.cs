using FluentValidation;

namespace FileService.Api.Dtos
{
    /// <summary>
    /// file exists request dto
    /// </summary>
    public record CheckFileExistsRequestDto
    {
        public long FileSizeBytes { get; set; }
        public string FileHash{ get; set; }
    }


    public class CheckFileExistsRequestDtoValidator : AbstractValidator<CheckFileExistsRequestDto>
    {
        public CheckFileExistsRequestDtoValidator()
        {

            RuleFor(x => x.FileHash)
                .NotEmpty().WithMessage("FileHash is required.")
                .Length(64).WithMessage("FileHash is not correct.");

            RuleFor(x => x.FileSizeBytes)               
                .GreaterThan(0).WithMessage("FileSizeBytes must be at greater than 0.");
        }
    }
}
