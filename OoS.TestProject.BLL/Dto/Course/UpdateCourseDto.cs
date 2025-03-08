using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OoS.TestProject.BLL.Dto.Course
{
    public class UpdateCourseDto
    {
        public string Title { get; set; }
        public int Credits { get; set; }
        public int TeacherId { get; set; }
    }
}
