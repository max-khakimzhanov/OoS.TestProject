using FluentValidation.TestHelper;
using OoS.TestProject.BLL.Dto.User;
using OoS.TestProject.BLL.Validation.Validators.DtoValidators.User;

namespace OoS.TestProject.NUnitTest.ValidatorTests
{
    [TestFixture]
    public class LoginDtoValidatorTests
    {
        private LoginDtoValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new LoginDtoValidator();
        }

        [Test]
        public void Should_Pass_When_All_Fields_Are_Valid()
        {
            // Arrange
            var model = new LoginDto
            {
                UserName = "testuser",
                Password = "password123"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void Should_Have_Error_When_UserName_Is_Empty()
        {
            // Arrange
            var model = new LoginDto
            {
                UserName = "",
                Password = "password123"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserName);
        }

        [Test]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            // Arrange
            var model = new LoginDto
            {
                UserName = "testuser",
                Password = ""
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
}
