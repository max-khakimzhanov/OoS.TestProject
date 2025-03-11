using FluentValidation.TestHelper;
using OoS.TestProject.BLL.Dto.User;
using OoS.TestProject.BLL.Validation.Validators.DtoValidators.User;

namespace OoS.TestProject.NUnitTest.ValidatorTests
{
    [TestFixture]
    public class RegisterDtoValidatorTests
    {
        private RegisterDtoValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new RegisterDtoValidator();
        }

        [Test]
        public void Should_Pass_When_All_Fields_Are_Valid()
        {
            // Arrange
            var model = new RegisterDto
            {
                UserName = "testuser",
                Password = "password123",
                PasswordConfirm = "password123"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void Should_Have_Error_When_Username_Is_Empty()
        {
            // Arrange
            var model = new RegisterDto
            {
                UserName = "",
                Password = "password123",
                PasswordConfirm = "password123"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserName);
        }

        [Test]
        public void Should_Have_Error_When_Password_Is_Too_Short()
        {
            // Arrange
            var model = new RegisterDto
            {
                UserName = "testuser",
                Password = "123",
                PasswordConfirm = "123"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void Should_Have_Error_When_Password_And_PasswordConfirm_Do_Not_Match()
        {
            // Arrange
            var model = new RegisterDto
            {
                UserName = "testuser",
                Password = "password123",
                PasswordConfirm = "differentPassword"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PasswordConfirm);
        }
    }
}
