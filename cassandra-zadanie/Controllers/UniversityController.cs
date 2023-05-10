using app3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace app3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UniversityController : ControllerBase
    {
        private readonly DBC _dbc;

        public UniversityController(DBC dbc)
        {
            _dbc = dbc;
        }

        [HttpGet()]
        public ActionResult<List<University>> GetUniversities()
        {
            return _dbc.GetUniversities();
        }

        [HttpPost()]
        public ActionResult AddUniversity(University university)
        {
            _dbc.AddUniversity(university);
            return Ok();
        }

        [HttpGet("api/{name}/department")]
        public ActionResult<List<object>> GetDepartments(string name)
        {
            return _dbc.GetDepartments(name);
        }
        [HttpPost("api/{name}/department")]
        public ActionResult AddDepartments(string name, [FromBody]UniversityDepartment ud)
        {
            _dbc.AddDepartment(name, ud);
            return Ok();
        }
        [HttpGet("api/{name}/{department}/field")]
        public ActionResult<List<object>> GetDepartmentFields(string name, string department)
        {
            return _dbc.GetDepartmentFields(name, department);
        }
        [HttpPost("api/{name}/{department}/field")]
        public ActionResult AddDepartmentField(string name, string department, [FromBody] UniversityDepartmentField udf)
        {
            _dbc.AddDepartmentField(name, department, udf);
            return Ok();
        }
        [HttpPost("searchbyname")]
        public ActionResult<List<object>> SearchByName([FromBody] string name)
        {
            
            return Ok(_dbc.SearchByName(name));
        }
        [HttpPost("searchbygroup")]
        public ActionResult<HashSet<string>> SearchByGroup([FromBody] string group)
        {
            return Ok(_dbc.SearchByGroup(group));
        }
    }
}
