using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.Dto.Course;
using OoS.TestProject.BLL.MediatR.CourseEntity.GetAll;
using OoS.TestProject.DAL.Repositories.Interfaces;
using System.Linq.Expressions;
using CourseEntity = OoS.TestProject.DAL.Entities.Course;

namespace OoS.TestProject.NUnitTest.MediatRTests.Course.GetAll
{
    [TestFixture]
    public class GetAllCourseHandlerTests
    {
        private Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<GetAllCourseHandler>> _loggerMock;
        private GetAllCourseHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<GetAllCourseHandler>>();
            _handler = new GetAllCourseHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnCourses_WhenCoursesExist()
        {
            // Arrange
            var courses = new List<CourseEntity> { new CourseEntity { Id = 1, Title = "Math", Credits = 3, TeacherId = 10 } };
            var courseDtos = new List<CourseDto> { new CourseDto { Id = 1, Title = "Math", Credits = 3, TeacherId = 10 } };

            _repositoryWrapperMock.Setup(r => r.CourseRepository
                .GetAllAsync(
                    It.IsAny<Expression<Func<CourseEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<CourseEntity>, IIncludableQueryable<CourseEntity, object>>>()))
                .ReturnsAsync(courses);
            _mapperMock.Setup(m => m.Map<IEnumerable<CourseDto>>(courses)).Returns(courseDtos);

            var query = new GetAllCourseQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(courseDtos.Count, result.Value.Count());
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenNoCoursesExist()
        {
            // Arrange
            _repositoryWrapperMock.Setup(r => r.CourseRepository
                .GetAllAsync(
                    It.IsAny<Expression<Func<CourseEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<CourseEntity>, IIncludableQueryable<CourseEntity, object>>>()))
                .ReturnsAsync((IEnumerable<CourseEntity>)null);

            var query = new GetAllCourseQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("No courses found.", result.Errors.First().Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenExceptionIsThrown()
        {
            // Arrange
            _repositoryWrapperMock.Setup(r => r.CourseRepository
                .GetAllAsync(
                    It.IsAny<Expression<Func<CourseEntity, bool>>>(), 
                    It.IsAny<Func<IQueryable<CourseEntity>, IIncludableQueryable<CourseEntity, object>>>()))
                .ThrowsAsync(new Exception("Database error"));

            var query = new GetAllCourseQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("An error occurred while retrieving courses.", result.Errors.First().Message);
        }
    }
}
