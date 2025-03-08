using FluentValidation.TestHelper;
using OoS.TestProject.BLL.Dto.Student;
using OoS.TestProject.BLL.Validation.Validators.DtoValidators.Student;

namespace OoS.TestProject.Tests.Validators
{
    [TestFixture]
    public class CreateStudentDtoValidatorTests
    {
        private CreateStudentDtoValidator _createStudentDtoValidator;

        [SetUp]
        public void Setup()
        {
            _createStudentDtoValidator = new CreateStudentDtoValidator();
        }

        [Test]
        public void CreateStudentDto_ValidData_ShouldPass()
        {
            // Arrange
            var model = new CreateStudentDto { Name = "John Doe", Email = "john.doe@example.com" };

            // Act
            var result = _createStudentDtoValidator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void CreateStudentDto_InvalidEmail_ShouldFail()
        {
            // Arrange
            var model = new CreateStudentDto { Name = "John Doe", Email = "invalid-email" };

            // Act
            var result = _createStudentDtoValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("Invalid email format.");
        }
    }
}
