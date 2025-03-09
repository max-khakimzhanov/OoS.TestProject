using Microsoft.AspNetCore.Mvc;
using OoS.TestProject.BLL.Dto.Teacher;
using OoS.TestProject.BLL.MediatR.Teacher.Create;
using OoS.TestProject.BLL.MediatR.Teacher.Delete;
using OoS.TestProject.BLL.MediatR.Teacher.GetAll;
using OoS.TestProject.BLL.MediatR.Teacher.GetById;
using OoS.TestProject.BLL.MediatR.Teacher.Update;

namespace OoS.TestProject.WebApi.Controllers
{
    public class TeacherController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return HandleResult(await Mediator.Send(new GetAllTeacherQuery()));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetByIdTeacherQuery(id)));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTeacherDto teacher)
        {
            return HandleResult(await Mediator.Send(new CreateTeacherCommand(teacher)));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateTeacherDto teacher)
        {
            return HandleResult(await Mediator.Send(new UpdateTeacherCommand(id, teacher)));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new DeleteTeacherCommand(id)));
        }
    }
}
