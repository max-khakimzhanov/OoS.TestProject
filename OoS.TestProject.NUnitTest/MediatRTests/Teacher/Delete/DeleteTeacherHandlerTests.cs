using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.DAL.Repositories.Interfaces;
using OoS.TestProject.BLL.MediatR.Teacher.Delete;
using System.Linq.Expressions;
using TeacherEntity = OoS.TestProject.DAL.Entities.Teacher;

namespace OoS.TestProject.NUnitTest.MediatRTests.Teacher.Delete
{
    [TestFixture]
    public class DeleteTeacherHandlerTests
    {
        private Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private Mock<ILogger<DeleteTeacherHandler>> _loggerMock;
        private DeleteTeacherHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILogger<DeleteTeacherHandler>>();
            _handler = new DeleteTeacherHandler(_repositoryWrapperMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenTeacherDeletedSuccessfully()
        {
            // Arrange
            var teacherId = 1;
            var teacher = new TeacherEntity { Id = teacherId };

            _repositoryWrapperMock.Setup(r => r.TeacherRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TeacherEntity, bool>>>(), null))
                .ReturnsAsync(teacher);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var command = new DeleteTeacherCommand(teacherId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _repositoryWrapperMock.Verify(r => r.TeacherRepository.Delete(teacher), Times.Once);
            _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenTeacherNotFound()
        {
            // Arrange
            var teacherId = 1;
            _repositoryWrapperMock.Setup(r => r.TeacherRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TeacherEntity, bool>>>(), null))
                .ReturnsAsync((TeacherEntity)null);
            var command = new DeleteTeacherCommand(teacherId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsFailed);
            Assert.AreEqual($"No teacher found by entered Id - {teacherId}", result.Errors[0].Message);
            _repositoryWrapperMock.Verify(r => r.TeacherRepository.Delete(It.IsAny<TeacherEntity>()), Times.Never);
            _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
        {
            // Arrange
            var teacherId = 1;
            var teacher = new TeacherEntity { Id = teacherId };

            _repositoryWrapperMock.Setup(r => r.TeacherRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TeacherEntity, bool>>>(), null))
                .ReturnsAsync(teacher);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            var command = new DeleteTeacherCommand(teacherId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsFailed);
            Assert.AreEqual("Failed to delete teacher", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenExceptionThrown()
        {
            // Arrange
            var teacherId = 1;

            _repositoryWrapperMock.Setup(r => r.TeacherRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TeacherEntity, bool>>>(), null))
                .ThrowsAsync(new Exception("Database error"));

            var command = new DeleteTeacherCommand(teacherId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsFailed);
            Assert.AreEqual("An unexpected error occurred.", result.Errors[0].Message);
        }
    }
}
