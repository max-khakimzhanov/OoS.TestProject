using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.Dto.Student;
using OoS.TestProject.BLL.MediatR.Student.GetById;
using OoS.TestProject.DAL.Repositories.Interfaces;
using System.Linq.Expressions;
using StudentEntity = OoS.TestProject.DAL.Entities.Student;

namespace OoS.TestProject.NUnitTest.MediatRTests.Student.GetById
{
    [TestFixture]
    public class GetByIdStudentHandlerTests
    {
        private Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<GetByIdStudentHandler>> _mockLogger;
        private GetByIdStudentHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<GetByIdStudentHandler>>();

            _handler = new GetByIdStudentHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnStudent_WhenStudentExists()
        {
            // Arrange
            var student = new StudentEntity { Id = 1, Name = "John Doe" };
            var studentDto = new StudentDto { Id = 1, Name = "John Doe" };

            _mockRepositoryWrapper.Setup(repo => repo.StudentRepository
                .GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StudentEntity, bool>>>(), 
                    It.IsAny<Func<IQueryable<StudentEntity>, IIncludableQueryable<StudentEntity, object>>>()))
                .ReturnsAsync(student);

            _mockMapper.Setup(m => m.Map<StudentDto>(student))
                .Returns(studentDto);

            var query = new GetByIdStudentQuery(1);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, result.Value.Id);
            Assert.AreEqual("John Doe", result.Value.Name);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenStudentDoesNotExist()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.StudentRepository
                .GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StudentEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<StudentEntity>, IIncludableQueryable<StudentEntity, object>>>()))
                .ReturnsAsync((StudentEntity)null);

            var query = new GetByIdStudentQuery(99);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("No student found by entered Id - 99", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenExceptionIsThrown()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.StudentRepository
                .GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StudentEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<StudentEntity>, IIncludableQueryable<StudentEntity, object>>>()))
                .ThrowsAsync(new Exception("Database error"));

            var query = new GetByIdStudentQuery(1);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("An unexpected error occurred.", result.Errors[0].Message);
        }
    }
}
