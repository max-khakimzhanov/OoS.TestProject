using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.Dto.Teacher;
using OoS.TestProject.BLL.MediatR.Teacher.Update;
using OoS.TestProject.DAL.Repositories.Interfaces;
using System.Linq.Expressions;
using TeacherEntity = OoS.TestProject.DAL.Entities.Teacher;

namespace OoS.TestProject.NUnitTest.MediatRTests.Teacher.Update
{
    [TestFixture]
    public class UpdateTeacherHandlerTests
    {
        private Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<UpdateTeacherHandler>> _mockLogger;
        private UpdateTeacherHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UpdateTeacherHandler>>();

            _handler = new UpdateTeacherHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Handle_ShouldUpdateTeacher_WhenStudentExists()
        {
            // Arrange
            var teacherId = 1;
            var updateDto = new UpdateTeacherDto { Name = "Updated Name", Email = "updated@email.com" };
            var existingTeacher = new TeacherEntity { Id = teacherId, Name = "Old Name", Email = "old@email.com" };
            var updatedTeacherDto = new TeacherDto { Id = teacherId, Name = "Updated Name", Email = "updated@email.com" };

            _mockRepositoryWrapper.Setup(repo => repo.TeacherRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TeacherEntity, bool>>>(), null))
                .ReturnsAsync(existingTeacher);

            _mockMapper.Setup(m => m.Map(updateDto, existingTeacher));

            _mockRepositoryWrapper.Setup(repo => repo.TeacherRepository.Update(existingTeacher));

            _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            _mockMapper.Setup(m => m.Map<TeacherDto>(existingTeacher))
                .Returns(updatedTeacherDto);

            var command = new UpdateTeacherCommand(teacherId, updateDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("Updated Name", result.Value.Name);
            Assert.AreEqual("updated@email.com", result.Value.Email);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenTeacherDoesNotExist()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.TeacherRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TeacherEntity, bool>>>(), null))
                .ReturnsAsync((TeacherEntity)null);

            var command = new UpdateTeacherCommand(99, new UpdateTeacherDto { Name = "New Name", Email = "new@email.com" });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Teacher with ID 99 not found.", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
        {
            // Arrange
            var teacherId = 1;
            var updateDto = new UpdateTeacherDto { Name = "Updated Name", Email = "updated@email.com" };
            var existingTeacher = new TeacherEntity { Id = teacherId, Name = "Old Name", Email = "old@email.com" };

            _mockRepositoryWrapper.Setup(repo => repo.TeacherRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TeacherEntity, bool>>>(), null))
                .ReturnsAsync(existingTeacher);

            _mockMapper.Setup(m => m.Map(updateDto, existingTeacher));

            _mockRepositoryWrapper.Setup(repo => repo.TeacherRepository.Update(existingTeacher));

            _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(0);

            var command = new UpdateTeacherCommand(teacherId, updateDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Failed to update teacher.", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenExceptionIsThrown()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.TeacherRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TeacherEntity, bool>>>(), null))
                .ThrowsAsync(new Exception("Database error"));

            var command = new UpdateTeacherCommand(1, new UpdateTeacherDto { Name = "New Name", Email = "new@email.com" });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("An unexpected error occurred.", result.Errors[0].Message);
        }
    }
}
