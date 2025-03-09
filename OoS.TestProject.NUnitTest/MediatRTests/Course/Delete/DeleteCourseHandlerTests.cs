using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.MediatR.CourseEntity.Delete;
using OoS.TestProject.DAL.Repositories.Interfaces;
using System.Linq.Expressions;
using CourseEntity = OoS.TestProject.DAL.Entities.Course;

namespace OoS.TestProject.NUnitTest.MediatRTests.Course.Delete
{
    [TestFixture]
    public class DeleteCourseHandlerTests
    {
        private Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private Mock<ILogger<DeleteCourseHandler>> _mockLogger;
        private DeleteCourseHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILogger<DeleteCourseHandler>>();

            _handler = new DeleteCourseHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenCourseDeletedSuccessfully()
        {
            // Arrange
            var courseId = 1;
            var course = new CourseEntity { Id = courseId, Title = "Sample Course", Credits = 3 };

            _mockRepositoryWrapper.Setup(repo => repo.CourseRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CourseEntity, bool>>>(), null))
                .ReturnsAsync(course);

            _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(new DeleteCourseCommand(courseId), CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenCourseNotFound()
        {
            // Arrange
            var courseId = 99; // Invalid Id
            _mockRepositoryWrapper.Setup(repo =>repo.CourseRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CourseEntity, bool>>>(), null))
                .ReturnsAsync((CourseEntity)null);

            // Act
            var result = await _handler.Handle(new DeleteCourseCommand(courseId), CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual($"No course found by entered Id - {courseId}", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
        {
            // Arrange
            var courseId = 2;
            var course = new CourseEntity { Id = courseId, Title = "Another Course", Credits = 4 };

            _mockRepositoryWrapper.Setup(repo => repo.CourseRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CourseEntity, bool>>>(), null))
                .ReturnsAsync(course);
            _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0); // Imitating failure

            // Act
            var result = await _handler.Handle(new DeleteCourseCommand(courseId), CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Failed to delete course", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenExceptionIsThrown()
        {
            // Arrange
            var courseId = 3;
            _mockRepositoryWrapper.Setup(repo => repo.CourseRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CourseEntity, bool>>>(), null))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(new DeleteCourseCommand(courseId), CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("An unexpected error occurred.", result.Errors[0].Message);
        }
    }
}