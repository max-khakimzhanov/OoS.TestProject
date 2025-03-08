using FluentValidation.TestHelper;
using OoS.TestProject.BLL.Dto.Teacher;
using OoS.TestProject.BLL.Validation.Validators.DtoValidators.Teacher;

namespace OoS.TestProject.Tests.Validators
{
    [TestFixture]
    public class UpdateTeacherDtoValidatorTests
    {
        private UpdateTeacherDtoValidator _updateTeacherDtoValidator;

        [SetUp]
        public void SetUp()
        {
            _updateTeacherDtoValidator = new UpdateTeacherDtoValidator();
        }

        [Test]
        public void Should_HaveError_When_NameIsEmpty()
        {
            // Arrange
            var model = new UpdateTeacherDto { Name = "", Email = "teacher@email.com" };

            // Act
            var result = _updateTeacherDtoValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Name is required.");
        }

        [Test]
        public void Should_HaveError_When_EmailIsInvalid()
        {
            // Arrange
            var model = new UpdateTeacherDto { Name = "Valid Name", Email = "invalid-email" };

            // Act
            var result = _updateTeacherDtoValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("Invalid email format.");
        }

        [Test]
        public void Should_NotHaveError_When_ModelIsValid()
        {
            // Arrange
            var model = new UpdateTeacherDto { Name = "Valid Name", Email = "teacher@email.com" };

            // Act
            var result = _updateTeacherDtoValidator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
