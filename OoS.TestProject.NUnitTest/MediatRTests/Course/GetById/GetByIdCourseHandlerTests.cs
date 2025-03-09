using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.Dto.Course;
using OoS.TestProject.BLL.MediatR.CourseEntity.GetById;
using OoS.TestProject.DAL.Repositories.Interfaces;
using System.Linq.Expressions;
using CourseEntity = OoS.TestProject.DAL.Entities.Course;

namespace OoS.TestProject.NUnitTest.MediatRTests.Course.GetById
{
    [TestFixture]
    public class GetByIdCourseHandlerTests
    {
        private Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<GetByIdCourseHandler>> _mockLogger;
        private GetByIdCourseHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<GetByIdCourseHandler>>();

            _handler = new GetByIdCourseHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnCourse_WhenCourseExists()
        {
            // Arrange
            var course = new CourseEntity { Id = 1, Title = "Mathematics" };
            var courseDto = new CourseDto { Id = 1, Title = "Mathematics" };

            _mockRepositoryWrapper.Setup(repo => repo.CourseRepository
                .GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<CourseEntity, bool>>>(), 
                    It.IsAny<Func<IQueryable<CourseEntity>, IIncludableQueryable<CourseEntity, object>>>()))
                .ReturnsAsync(course);

            _mockMapper.Setup(m => m.Map<CourseDto>(course))
                .Returns(courseDto);

            var query = new GetByIdCourseQuery(1);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, result.Value.Id);
            Assert.AreEqual("Mathematics", result.Value.Title);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenCourseDoesNotExist()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.CourseRepository
                .GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<CourseEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<CourseEntity>, IIncludableQueryable<CourseEntity, object>>>()))
                .ReturnsAsync((CourseEntity)null);

            var query = new GetByIdCourseQuery(99);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("No course found by entered Id - 99", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenExceptionIsThrown()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.CourseRepository
                 .GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<CourseEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<CourseEntity>, IIncludableQueryable<CourseEntity, object>>>()))
                .ThrowsAsync(new Exception("Database error"));

            var query = new GetByIdCourseQuery(1);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("An unexpected error occurred.", result.Errors[0].Message);
        }
    }
}
