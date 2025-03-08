using FluentValidation.TestHelper;
using OoS.TestProject.BLL.Dto.Student;
using OoS.TestProject.BLL.Validation.Validators.DtoValidators.Student;

namespace OoS.TestProject.Tests.Validators
{
    [TestFixture]
    public class UpdateStudentDtoValidatorTests
    {
        private UpdateStudentDtoValidator _updateStudentDtoValidator;

        [SetUp]
        public void SetUp()
        {
            _updateStudentDtoValidator = new UpdateStudentDtoValidator();
        }

        [Test]
        public void Should_HaveError_When_NameIsEmpty()
        {
            // Arrange
            var model = new UpdateStudentDto { Name = "", Email = "student@email.com" };

            // Act
            var result = _updateStudentDtoValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Name is required.");
        }

        [Test]
        public void Should_HaveError_When_EmailIsInvalid()
        {
            // Arrange
            var model = new UpdateStudentDto { Name = "Valid Name", Email = "invalid-email" };

            // Act
            var result = _updateStudentDtoValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("Invalid email format.");
        }

        [Test]
        public void Should_NotHaveError_When_ModelIsValid()
        {
            // Arrange
            var model = new UpdateStudentDto { Name = "Valid Name", Email = "student@email.com" };

            // Act
            var result = _updateStudentDtoValidator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
