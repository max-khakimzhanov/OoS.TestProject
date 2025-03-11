using FluentValidation;
using OoS.TestProject.BLL.Dto.User;

namespace OoS.TestProject.BLL.Validation.Validators.DtoValidators.User
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.PasswordConfirm)
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
        }
    }
}
