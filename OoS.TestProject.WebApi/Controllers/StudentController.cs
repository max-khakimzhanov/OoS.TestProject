using Microsoft.AspNetCore.Mvc;
using OoS.TestProject.BLL.Dto.Student;
using OoS.TestProject.BLL.MediatR.Student.Create;
using OoS.TestProject.BLL.MediatR.Student.Delete;
using OoS.TestProject.BLL.MediatR.Student.GetAll;
using OoS.TestProject.BLL.MediatR.Student.GetById;
using OoS.TestProject.BLL.MediatR.Student.Update;

namespace OoS.TestProject.WebApi.Controllers
{
    public class StudentController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return HandleResult(await Mediator.Send(new GetAllStudentQuery()));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetByIdStudentQuery(id)));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStudentDto student)
        {
            return HandleResult(await Mediator.Send(new CreateStudentCommand(student)));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStudentDto student)
        {
            return HandleResult(await Mediator.Send(new UpdateStudentCommand(id, student)));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new DeleteStudentCommand(id)));
        }
    }
}
