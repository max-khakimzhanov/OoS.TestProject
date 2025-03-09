using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.Dto.Teacher;
using OoS.TestProject.BLL.MediatR.Teacher.Create;
using OoS.TestProject.BLL.MediatR.TeacherEntity.Create;
using OoS.TestProject.DAL.Repositories.Interfaces;
using TeacherEntity = OoS.TestProject.DAL.Entities.Teacher;

namespace OoS.TestProject.NUnitTest.MediatRTests.Teacher.Create
{
    [TestFixture]
    public class CreateTeacherHandlerTests
    {
        private Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<CreateTeacherHandler>> _loggerMock;
        private CreateTeacherHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CreateTeacherHandler>>();
            _handler = new CreateTeacherHandler(
                _repositoryWrapperMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenTeacherCreatedSuccessfully()
        {
            // Arrange
            var teacherDto = new CreateTeacherDto { Name = "John Doe", Email = "john.doe@example.com" };
            var teacherEntity = new TeacherEntity { Id = 1, Name = "John Doe", Email = "john.doe@example.com" };
            var expectedResult = new TeacherDto { Id = 1, Name = "John Doe", Email = "john.doe@example.com" };
            _mapperMock.Setup(m => m.Map<TeacherEntity>(teacherDto)).Returns(teacherEntity);
            _repositoryWrapperMock.Setup(r => r.TeacherRepository.CreateAsync(teacherEntity)).ReturnsAsync(teacherEntity);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<TeacherDto>(teacherEntity)).Returns(expectedResult);

            var command = new CreateTeacherCommand(teacherDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(teacherEntity.Name, result.Value.Name);
            Assert.AreEqual(teacherEntity.Email, result.Value.Email);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenRequestIsNull()
        {
            // Arrange
            var command = new CreateTeacherCommand(null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsFailed);
            Assert.AreEqual("Teacher data is null", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenSaveChangesFails()
        {
            // Arrange
            var teacherDto = new CreateTeacherDto { Name = "Jane Doe", Email = "jane.doe@example.com" };
            var teacherEntity = new TeacherEntity { Id = 1, Name = "Jane Doe", Email = "jane.doe@example.com" };
            _mapperMock.Setup(m => m.Map<TeacherEntity>(teacherDto)).Returns(teacherEntity);
            _repositoryWrapperMock.Setup(r => r.TeacherRepository.CreateAsync(teacherEntity)).ReturnsAsync(teacherEntity);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            var command = new CreateTeacherCommand(teacherDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsFailed);
            Assert.AreEqual("Failed to create a teacher", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenExceptionThrown()
        {
            // Arrange
            var teacherDto = new CreateTeacherDto { Name = "Error Teacher", Email = "error@example.com" };
            _mapperMock.Setup(m => m.Map<TeacherEntity>(It.IsAny<CreateTeacherDto>()))
                .Throws(new Exception("Unexpected error"));

            var command = new CreateTeacherCommand(teacherDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsFailed);
            Assert.AreEqual("An unexpected error occurred while creating the teacher.", result.Errors[0].Message);
        }
    }
}