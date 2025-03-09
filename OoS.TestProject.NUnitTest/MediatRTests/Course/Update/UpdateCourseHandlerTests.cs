using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.Dto.Course;
using OoS.TestProject.BLL.MediatR.CourseEntity.Update;
using OoS.TestProject.DAL.Repositories.Interfaces;
using System.Linq.Expressions;
using CourseEntity = OoS.TestProject.DAL.Entities.Course;

namespace OoS.TestProject.NUnitTest.MediatRTests.Course.Update
{
    [TestFixture]
    public class UpdateCourseHandlerTests
    {
        private Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<UpdateCourseHandler>> _mockLogger;
        private UpdateCourseHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UpdateCourseHandler>>();

            _handler = new UpdateCourseHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Handle_ShouldUpdateCourse_WhenCourseExists()
        {
            // Arrange
            var courseId = 1;
            var updateDto = new UpdateCourseDto { Title = "Updated Course", Credits = 4, TeacherId = 2 };
            var existingCourse = new CourseEntity { Id = courseId, Title = "Old Course", Credits = 3, TeacherId = 1 };
            var updatedCourseDto = new CourseDto { Id = courseId, Title = "Updated Course", Credits = 4, TeacherId = 2 };

            _mockRepositoryWrapper.Setup(repo => repo.CourseRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CourseEntity, bool>>>(), null))
                .ReturnsAsync(existingCourse);

            _mockMapper.Setup(m => m.Map(updateDto, existingCourse));

            _mockRepositoryWrapper.Setup(repo => repo.CourseRepository.Update(existingCourse));

            _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            _mockMapper.Setup(m => m.Map<CourseDto>(existingCourse))
                .Returns(updatedCourseDto);

            var command = new UpdateCourseCommand(courseId, updateDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("Updated Course", result.Value.Title);
            Assert.AreEqual(4, result.Value.Credits);
            Assert.AreEqual(2, result.Value.TeacherId);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenCourseDoesNotExist()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.CourseRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CourseEntity, bool>>>(), null))
                .ReturnsAsync((CourseEntity)null);

            var command = new UpdateCourseCommand(99, new UpdateCourseDto { Title = "New Title", Credits = 3, TeacherId = 1 });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Course with ID 99 not found.", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
        {
            // Arrange
            var courseId = 1;
            var updateDto = new UpdateCourseDto { Title = "Updated Course", Credits = 4, TeacherId = 2 };
            var existingCourse = new CourseEntity { Id = courseId, Title = "Old Course", Credits = 3, TeacherId = 1 };

            _mockRepositoryWrapper.Setup(repo => repo.CourseRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CourseEntity, bool>>>(), null))
                .ReturnsAsync(existingCourse);

            _mockMapper.Setup(m => m.Map(updateDto, existingCourse));

            _mockRepositoryWrapper.Setup(repo => repo.CourseRepository.Update(existingCourse));

            _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(0);

            var command = new UpdateCourseCommand(courseId, updateDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Failed to update course.", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenExceptionIsThrown()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.CourseRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CourseEntity, bool>>>(), null))
                .ThrowsAsync(new Exception("Database error"));

            var command = new UpdateCourseCommand(1, new UpdateCourseDto { Title = "New Title", Credits = 3, TeacherId = 1 });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("An unexpected error occurred.", result.Errors[0].Message);
        }
    }
}
