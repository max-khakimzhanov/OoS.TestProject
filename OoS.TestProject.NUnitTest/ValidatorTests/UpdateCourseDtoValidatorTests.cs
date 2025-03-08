using FluentValidation.TestHelper;
using OoS.TestProject.BLL.Dto.Course;
using OoS.TestProject.BLL.Validation.Validators.DtoValidators.Course;

namespace OoS.TestProject.Tests.Validators
{
    [TestFixture]
    public class UpdateCourseDtoValidatorTests
    {
        private UpdateCourseDtoValidator _updateCourseDtoValidator;

        [SetUp]
        public void SetUp()
        {
            _updateCourseDtoValidator = new UpdateCourseDtoValidator();
        }

        [Test]
        public void Should_HaveError_When_TitleIsEmpty()
        {
            // Arrange
            var model = new UpdateCourseDto { Title = string.Empty, Credits = 5, TeacherId = 1 };

            // Act
            var result = _updateCourseDtoValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title)
                .WithErrorMessage("Name is required.");
        }

        [Test]
        public void Should_HaveError_When_TitleIsTooShort()
        {
            // Arrange
            var model = new UpdateCourseDto { Title = "A", Credits = 5, TeacherId = 1 };

            // Act
            var result = _updateCourseDtoValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title)
                .WithErrorMessage("Name must be between 2 and 100 characters.");
        }

        [Test]
        public void Should_NotHaveError_When_ModelIsValid()
        {
            // Arrange
            var model = new UpdateCourseDto { Title = "Valid Title", Credits = 5, TeacherId = 1 };

            // Act
            var result = _updateCourseDtoValidator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void Should_HaveError_When_CreditsAreOutOfRange()
        {
            // Arrange
            var model = new UpdateCourseDto { Title = "Valid Title", Credits = 15, TeacherId = 1 };

            // Act
            var result = _updateCourseDtoValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Credits)
                .WithErrorMessage("Credits must be between 1 and 10.");
        }

        [Test]
        public void Should_HaveError_When_TeacherIdIsZero()
        {
            // Arrange
            var model = new UpdateCourseDto { Title = "Valid Title", Credits = 5, TeacherId = 0 };

            // Act
            var result = _updateCourseDtoValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TeacherId)
                .WithErrorMessage("Teacher ID must be greater than zero.");
        }
    }
}
