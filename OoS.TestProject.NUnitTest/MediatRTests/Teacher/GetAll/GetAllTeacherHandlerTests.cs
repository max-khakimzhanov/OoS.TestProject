using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.Dto.Teacher;
using OoS.TestProject.BLL.MediatR.Teacher.GetAll;
using OoS.TestProject.DAL.Repositories.Interfaces;
using System.Linq.Expressions;
using TeacherEntity = OoS.TestProject.DAL.Entities.Teacher;

namespace OoS.TestProject.NUnitTest.MediatRTests.Teacher.GetAll
{
    [TestFixture]
    public class GetAllTeacherHandlerTests
    {
        private Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<GetAllTeacherHandler>> _mockLogger;
        private GetAllTeacherHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<GetAllTeacherHandler>>();

            _handler = new GetAllTeacherHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnTeachers_WhenTeachersExist()
        {
            // Arrange
            var teachers = new List<TeacherEntity>
            {
                new TeacherEntity { Id = 1, Name = "John Doe" },
                new TeacherEntity { Id = 2, Name = "Jane Smith" }
            };

            var teacherDtos = new List<TeacherDto>
            {
                new TeacherDto { Id = 1, Name = "John Doe" },
                new TeacherDto { Id = 2, Name = "Jane Smith" }
            };

            _mockRepositoryWrapper.Setup(repo => repo.TeacherRepository
                .GetAllAsync(
                    It.IsAny<Expression<Func<TeacherEntity, bool>>>(), 
                    It.IsAny<Func<IQueryable<TeacherEntity>, IIncludableQueryable<TeacherEntity, object>>>()))
                .ReturnsAsync(teachers);

            _mockMapper.Setup(m => m.Map<IEnumerable<TeacherDto>>(teachers))
                .Returns(teacherDtos);

            // Act
            var result = await _handler.Handle(new GetAllTeacherQuery(), CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Value.Count());
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenNoTeachersExist()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.TeacherRepository
                .GetAllAsync(
                    It.IsAny<Expression<Func<TeacherEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TeacherEntity>, IIncludableQueryable<TeacherEntity, object>>>()))
                .ReturnsAsync((IEnumerable<TeacherEntity>)null);

            // Act
            var result = await _handler.Handle(new GetAllTeacherQuery(), CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("No teachers found.", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenExceptionIsThrown()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.TeacherRepository
                .GetAllAsync(
                    It.IsAny<Expression<Func<TeacherEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TeacherEntity>, IIncludableQueryable<TeacherEntity, object>>>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(new GetAllTeacherQuery(), CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("An error occurred while retrieving teachers.", result.Errors[0].Message);
        }
    }
}
