using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.Dto.Student;
using OoS.TestProject.BLL.MediatR.Student.Create;
using OoS.TestProject.BLL.MediatR.StudentEntity.Create;
using OoS.TestProject.DAL.Repositories.Interfaces;
using StudentEntity = OoS.TestProject.DAL.Entities.Student;

namespace OoS.TestProject.NUnitTest.MediatRTests.Student.Create
{
    [TestFixture]
    public class CreateStudentHandlerTests
    {
        private Mock<IRepositoryWrapper> _mockRepoWrapper;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<CreateStudentHandler>> _mockLogger;
        private CreateStudentHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepoWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<CreateStudentHandler>>();
            _handler = new CreateStudentHandler(_mockRepoWrapper.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Handle_ValidRequest_ShouldReturnSuccessResult()
        {
            // Arrange
            var createDto = new CreateStudentDto { Name = "John Doe", Email = "john.doe@example.com" };
            var studentEntity = new StudentEntity { Id = 1, Name = "John Doe", Email = "john.doe@example.com" };
            var expectedResult = new StudentDto { Id = 1, Name = "John Doe", Email = "john.doe@example.com" };

            _mockMapper.Setup(m => m.Map<StudentEntity>(createDto)).Returns(studentEntity);
            _mockRepoWrapper.Setup(r => r.StudentRepository.CreateAsync(studentEntity)).ReturnsAsync(studentEntity);
            _mockRepoWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<StudentDto>(studentEntity)).Returns(expectedResult);

            // Act
            var result = await _handler.Handle(new CreateStudentCommand(createDto), CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedResult, result.Value);
        }

        [Test]
        public async Task Handle_NullRequest_ShouldReturnFailResult()
        {
            // Arrange
            CreateStudentDto createDto = null;

            // Act
            var result = await _handler.Handle(new CreateStudentCommand(createDto), CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsFailed);
            Assert.AreEqual("Student data is null", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_SaveChangesFails_ShouldReturnFailResult()
        {
            // Arrange
            var createDto = new CreateStudentDto { Name = "John Doe", Email = "john.doe@example.com" };
            var studentEntity = new StudentEntity { Id = 1, Name = "John Doe", Email = "john.doe@example.com" };

            _mockMapper.Setup(m => m.Map<StudentEntity>(createDto)).Returns(studentEntity);
            _mockRepoWrapper.Setup(r => r.StudentRepository.CreateAsync(studentEntity)).ReturnsAsync(studentEntity);
            _mockRepoWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(new CreateStudentCommand(createDto), CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsFailed);
            Assert.AreEqual("Failed to create a student", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ExceptionThrown_ShouldReturnFailResult()
        {
            // Arrange
            var createDto = new CreateStudentDto { Name = "John Doe", Email = "john.doe@example.com" };

            _mockMapper.Setup(m => m.Map<StudentEntity>(createDto)).Throws(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(new CreateStudentCommand(createDto), CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsFailed);
            Assert.AreEqual("An unexpected error occurred while creating the student.", result.Errors[0].Message);
        }
    }
}
