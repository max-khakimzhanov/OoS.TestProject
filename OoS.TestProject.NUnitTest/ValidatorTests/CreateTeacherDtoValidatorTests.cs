using FluentValidation.TestHelper;
using OoS.TestProject.BLL.Dto.Teacher;
using OoS.TestProject.BLL.Validation.Validators.DtoValidators.Teacher;

namespace OoS.TestProject.Tests.Validators
{
    [TestFixture]
    public class CreateTeacherDtoValidatorTests
    {
        private CreateTeacherDtoValidator _createTeacherDtoValidator;

        [SetUp]
        public void Setup()
        {
            _createTeacherDtoValidator = new CreateTeacherDtoValidator();
        }

        [Test]
        public void CreateTeacherDto_ValidData_ShouldPass()
        {
            // Arrange
            var model = new CreateTeacherDto { Name = "Jane Doe", Email = "jane.doe@example.com" };

            // Act
            var result = _createTeacherDtoValidator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void CreateTeacherDto_MissingName_ShouldFail()
        {
            // Arrange
            var model = new CreateTeacherDto { Name = "", Email = "jane.doe@example.com" };

            // Act
            var result = _createTeacherDtoValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Name is required.");
        }
    }
}
