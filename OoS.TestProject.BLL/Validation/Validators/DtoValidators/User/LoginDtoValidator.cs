using FluentValidation;
using OoS.TestProject.BLL.Dto.User;

namespace OoS.TestProject.BLL.Validation.Validators.DtoValidators.User
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
