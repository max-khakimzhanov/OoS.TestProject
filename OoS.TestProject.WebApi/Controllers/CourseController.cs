using Microsoft.AspNetCore.Mvc;
using OoS.TestProject.BLL.Dto.Course;
using OoS.TestProject.BLL.MediatR.CourseEntity.Create;
using OoS.TestProject.BLL.MediatR.CourseEntity.Delete;
using OoS.TestProject.BLL.MediatR.CourseEntity.GetAll;
using OoS.TestProject.BLL.MediatR.CourseEntity.GetById;
using OoS.TestProject.BLL.MediatR.CourseEntity.Update;

namespace OoS.TestProject.WebApi.Controllers
{
    public class CourseController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return HandleResult(await Mediator.Send(new GetAllCourseQuery()));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetByIdCourseQuery(id)));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto course)
        {
            return HandleResult(await Mediator.Send(new CreateCourseCommand(course)));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCourseDto course)
        {
            return HandleResult(await Mediator.Send(new UpdateCourseCommand(id, course)));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new DeleteCourseCommand(id)));
        }
    }
}
