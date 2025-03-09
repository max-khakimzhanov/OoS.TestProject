using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.Dto.Course;
using OoS.TestProject.BLL.MediatR.CourseEntity.Create;
using OoS.TestProject.DAL.Repositories.Interfaces;
using CourseEntity = OoS.TestProject.DAL.Entities.Course;

namespace OoS.TestProject.NUnitTest.MediatRTests.Course.Create
{
    [TestFixture]
    public class CreateCourseHandlerTests
    {
        private Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<CreateCourseHandler>> _mockLogger;
        private CreateCourseHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<CreateCourseHandler>>();

            _handler = new CreateCourseHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenCourseDataIsNull()
        {
            // Arrange
            var command = new CreateCourseCommand(null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsFailed);
            Assert.AreEqual("Course data is null", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenCourseCreatedSuccessfully()
        {
            // Arrange
            var createDto = new CreateCourseDto { Title = "Test Course", Credits = 3, TeacherId = 1 };
            var command = new CreateCourseCommand(createDto);
            var courseEntity = new CourseEntity { Id = 1, Title = "Test Course", Credits = 3, TeacherId = 1 };
            var courseDto = new CourseDto { Id = 1, Title = "Test Course", Credits = 3, TeacherId = 1 };

            _mockMapper.Setup(m => m.Map<CourseEntity>(createDto)).Returns(courseEntity);
            _mockRepositoryWrapper.Setup(r => r.CourseRepository.CreateAsync(courseEntity)).ReturnsAsync(courseEntity);
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<CourseDto>(courseEntity)).Returns(courseDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(courseDto, result.Value);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
        {
            // Arrange
            var createDto = new CreateCourseDto { Title = "Test Course", Credits = 3, TeacherId = 1 };
            var command = new CreateCourseCommand(createDto);
            var courseEntity = new CourseEntity { Id = 1, Title = "Test Course", Credits = 3, TeacherId = 1 };

            _mockMapper.Setup(m => m.Map<CourseEntity>(createDto)).Returns(courseEntity);
            _mockRepositoryWrapper.Setup(r => r.CourseRepository.CreateAsync(courseEntity)).ReturnsAsync(courseEntity);
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsFailed);
            Assert.AreEqual("Failed to create a course", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenExceptionIsThrown()
        {
            // Arrange
            var createDto = new CreateCourseDto { Title = "Test Course", Credits = 3, TeacherId = 1 };
            var command = new CreateCourseCommand(createDto);

            _mockMapper.Setup(m => m.Map<CourseEntity>(createDto)).Throws(new Exception("Unexpected error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsFailed);
            Assert.AreEqual("An unexpected error occurred while creating the course.", result.Errors[0].Message);
        }
    }
}
