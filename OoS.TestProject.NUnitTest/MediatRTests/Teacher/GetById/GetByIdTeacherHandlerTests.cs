using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.Dto.Teacher;
using OoS.TestProject.BLL.MediatR.Teacher.GetById;
using OoS.TestProject.DAL.Repositories.Interfaces;
using System;
using System.Linq.Expressions;
using TeacherEntity = OoS.TestProject.DAL.Entities.Teacher;

namespace OoS.TestProject.NUnitTest.MediatRTests.Teacher.GetById
{
    [TestFixture]
    public class GetByIdTeacherHandlerTests
    {
        private Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<GetByIdTeacherHandler>> _mockLogger;
        private GetByIdTeacherHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<GetByIdTeacherHandler>>();

            _handler = new GetByIdTeacherHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnTeacher_WhenTeacherExists()
        {
            // Arrange
            var teacher = new TeacherEntity { Id = 1, Name = "Jane Smith" };
            var teacherDto = new TeacherDto { Id = 1, Name = "Jane Smith" };

            _mockRepositoryWrapper.Setup(repo => repo.TeacherRepository
                .GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TeacherEntity, bool>>>(), 
                    It.IsAny<Func<IQueryable<TeacherEntity>, IIncludableQueryable<TeacherEntity, object>>>()))
                .ReturnsAsync(teacher);

            _mockMapper.Setup(m => m.Map<TeacherDto>(teacher))
                .Returns(teacherDto);

            var query = new GetByIdTeacherQuery(1);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, result.Value.Id);
            Assert.AreEqual("Jane Smith", result.Value.Name);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenTeacherDoesNotExist()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.TeacherRepository
                .GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TeacherEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TeacherEntity>, IIncludableQueryable<TeacherEntity, object>>>()))
                .ReturnsAsync((TeacherEntity)null);

            var query = new GetByIdTeacherQuery(99);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("No teacher found by entered Id - 99", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenExceptionIsThrown()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.TeacherRepository
                .GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TeacherEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TeacherEntity>, IIncludableQueryable<TeacherEntity, object>>>()))
                .ThrowsAsync(new Exception("Database error"));

            var query = new GetByIdTeacherQuery(1);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("An unexpected error occurred.", result.Errors[0].Message);
        }
    }
}
