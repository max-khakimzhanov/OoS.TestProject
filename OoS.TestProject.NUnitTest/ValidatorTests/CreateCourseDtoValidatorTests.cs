using FluentValidation.TestHelper;
using OoS.TestProject.BLL.Dto.Course;
using OoS.TestProject.BLL.Validation.Validators.DtoValidators.Course;

namespace OoS.TestProject.Tests.Validators
{
    [TestFixture]
    public class CreateCourseDtoValidatorTests
    {
        private CreateCourseDtoValidator _createCourseDtoValidator;

        [SetUp]
        public void Setup()
        {
            _createCourseDtoValidator = new CreateCourseDtoValidator();
        }

        [Test]
        public void CreateCourseDto_ValidData_ShouldPass()
        {
            // Arrange
            var model = new CreateCourseDto { Title = "Math", Credits = 5, TeacherId = 1 };

            // Act
            var result = _createCourseDtoValidator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void CreateCourseDto_InvalidCredits_ShouldFail()
        {
            // Arrange
            var model = new CreateCourseDto { Title = "Math", Credits = 20, TeacherId = 1 };

            // Act
            var result = _createCourseDtoValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Credits)
                .WithErrorMessage("Credits must be between 1 and 10.");
        }
    }
}
