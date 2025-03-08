using FluentValidation;
using OoS.TestProject.BLL.Dto.Course;

namespace OoS.TestProject.BLL.Validation.Validators.DtoValidators.Course
{
    public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
    {
        public CreateCourseDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");
            RuleFor(course => course.Credits)
                .InclusiveBetween(1, 10)
                .WithMessage("Credits must be between 1 and 10.");
            RuleFor(x => x.TeacherId)
                .GreaterThan(0).WithMessage("Teacher ID must be greater than zero.");
        }
    }
}
