using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.MediatR.Student.Delete;
using OoS.TestProject.DAL.Repositories.Interfaces;
using System.Linq.Expressions;
using StudentEntity = OoS.TestProject.DAL.Entities.Student;

namespace OoS.TestProject.NUnitTest.MediatRTests.Student.Delete
{
    [TestFixture]
    public class DeleteStudentHandlerTests
    {
        private Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private Mock<ILogger<DeleteStudentHandler>> _loggerMock;
        private DeleteStudentHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILogger<DeleteStudentHandler>>();
            _handler = new DeleteStudentHandler(_repositoryWrapperMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenStudentDeletedSuccessfully()
        {
            // Arrange
            var student = new StudentEntity { Id = 1 };
            _repositoryWrapperMock.Setup(r => r.StudentRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StudentEntity, bool>>>(), null))
                .ReturnsAsync(student);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(new DeleteStudentCommand(1), CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _repositoryWrapperMock.Verify(r => r.StudentRepository.Delete(student), Times.Once);
            _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenStudentNotFound()
        {
            // Arrange
            _repositoryWrapperMock.Setup(r => r.StudentRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StudentEntity, bool>>>(), null))
                .ReturnsAsync((StudentEntity)null);

            // Act
            var result = await _handler.Handle(new DeleteStudentCommand(1), CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Errors[0].Message, Is.EqualTo("No student found by entered Id - 1"));
            _repositoryWrapperMock.Verify(r => r.StudentRepository.Delete(It.IsAny<StudentEntity>()), Times.Never);
            _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenSaveChangesFails()
        {
            // Arrange
            var student = new StudentEntity { Id = 1 };
            _repositoryWrapperMock.Setup(r => r.StudentRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StudentEntity, bool>>>(), null))
                .ReturnsAsync(student);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(new DeleteStudentCommand(1), CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Errors[0].Message, Is.EqualTo("Failed to delete student"));
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
        {
            // Arrange
            _repositoryWrapperMock.Setup(r => r.StudentRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StudentEntity, bool>>>(), null))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(new DeleteStudentCommand(1), CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Errors[0].Message, Is.EqualTo("An unexpected error occurred."));
        }
    }
}
