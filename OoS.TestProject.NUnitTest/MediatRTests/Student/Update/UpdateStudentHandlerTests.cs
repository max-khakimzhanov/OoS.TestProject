using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using OoS.TestProject.BLL.Dto.Student;
using OoS.TestProject.BLL.MediatR.Student.Update;
using OoS.TestProject.DAL.Repositories.Interfaces;
using System.Linq.Expressions;
using StudentEntity = OoS.TestProject.DAL.Entities.Student;

namespace OoS.TestProject.NUnitTest.MediatRTests.Student.Update
{
    [TestFixture]
    public class UpdateStudentHandlerTests
    {
        private Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<UpdateStudentHandler>> _mockLogger;
        private UpdateStudentHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UpdateStudentHandler>>();

            _handler = new UpdateStudentHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Handle_ShouldUpdateStudent_WhenStudentExists()
        {
            // Arrange
            var studentId = 1;
            var updateDto = new UpdateStudentDto { Name = "Updated Name", Email = "updated@email.com" };
            var existingStudent = new StudentEntity { Id = studentId, Name = "Old Name", Email = "old@email.com" };
            var updatedStudentDto = new StudentDto { Id = studentId, Name = "Updated Name", Email = "updated@email.com" };

            _mockRepositoryWrapper.Setup(repo => repo.StudentRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StudentEntity, bool>>>(), null))
                .ReturnsAsync(existingStudent);

            _mockMapper.Setup(m => m.Map(updateDto, existingStudent));

            _mockRepositoryWrapper.Setup(repo => repo.StudentRepository.Update(existingStudent));

            _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            _mockMapper.Setup(m => m.Map<StudentDto>(existingStudent))
                .Returns(updatedStudentDto);

            var command = new UpdateStudentCommand(studentId, updateDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("Updated Name", result.Value.Name);
            Assert.AreEqual("updated@email.com", result.Value.Email);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenStudentDoesNotExist()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.StudentRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StudentEntity, bool>>>(), null))
                .ReturnsAsync((StudentEntity)null);

            var command = new UpdateStudentCommand(99, new UpdateStudentDto { Name = "New Name", Email = "new@email.com" });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Student with ID 99 not found.", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
        {
            // Arrange
            var studentId = 1;
            var updateDto = new UpdateStudentDto { Name = "Updated Name", Email = "updated@email.com" };
            var existingStudent = new StudentEntity { Id = studentId, Name = "Old Name", Email = "old@email.com" };

            _mockRepositoryWrapper.Setup(repo => repo.StudentRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StudentEntity, bool>>>(), null))
                .ReturnsAsync(existingStudent);

            _mockMapper.Setup(m => m.Map(updateDto, existingStudent));

            _mockRepositoryWrapper.Setup(repo => repo.StudentRepository.Update(existingStudent));

            _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(0);

            var command = new UpdateStudentCommand(studentId, updateDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Failed to update student.", result.Errors[0].Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenExceptionIsThrown()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.StudentRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StudentEntity, bool>>>(), null))
                .ThrowsAsync(new Exception("Database error"));

            var command = new UpdateStudentCommand(1, new UpdateStudentDto { Name = "New Name", Email = "new@email.com" });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("An unexpected error occurred.", result.Errors[0].Message);
        }
    }
}
