using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.Dto.Student;
using OoS.TestProject.BLL.MediatR.Student.GetAll;
using OoS.TestProject.DAL.Repositories.Interfaces;
using StudentEntity = OoS.TestProject.DAL.Entities.Student;

namespace OoS.TestProject.NUnitTest.MediatRTests.Student.GetAll
{
    [TestFixture]
    public class GetAllStudentHandlerTests
    {
        private Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<GetAllStudentHandler>> _mockLogger;
        private GetAllStudentHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<GetAllStudentHandler>>();

            _handler = new GetAllStudentHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task Handle_ShouldReturnStudents_WhenStudentsExist()
        {
            // Arrange
            var students = new List<StudentEntity>
            {
                new StudentEntity { Id = 1, Name = "John Doe" },
                new StudentEntity { Id = 2, Name = "Jane Doe" }
            };

            var studentDtos = students.Select(s => new StudentDto { Id = s.Id, Name = s.Name }).ToList();

            _mockRepositoryWrapper.Setup(repo => repo.StudentRepository
                .GetAllAsync(
                    It.IsAny<Expression<Func<StudentEntity, bool>>>(), 
                    It.IsAny<Func<IQueryable<StudentEntity>, IIncludableQueryable<StudentEntity, object>>>()))
                .ReturnsAsync(students);

            _mockMapper.Setup(m => m.Map<IEnumerable<StudentDto>>(students)).Returns(studentDtos);

            // Act
            var result = await _handler.Handle(new GetAllStudentQuery(), CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Value.Count());
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenNoStudentsExist()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.StudentRepository
                .GetAllAsync(
                    It.IsAny<Expression<Func<StudentEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<StudentEntity>, IIncludableQueryable<StudentEntity, object>>>()))
                .ReturnsAsync((IEnumerable<StudentEntity>)null);

            // Act
            var result = await _handler.Handle(new GetAllStudentQuery(), CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("No students found.", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenExceptionIsThrown()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.StudentRepository
                .GetAllAsync(
                    It.IsAny<Expression<Func<StudentEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<StudentEntity>, IIncludableQueryable<StudentEntity, object>>>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(new GetAllStudentQuery(), CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("An error occurred while retrieving students.", result.Errors[0].Message);
        }
    }
}
