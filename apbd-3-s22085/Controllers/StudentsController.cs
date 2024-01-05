using apbd_3_s22085.Service;
using Microsoft.AspNetCore.Mvc;

namespace apbd_3_s22085.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentsController : ControllerBase
{
    private readonly ILogger<StudentsController> _logger;
    private readonly IDatabase<Student> _database;

    public StudentsController(
        ILogger<StudentsController> logger, 
        IDatabase<Student> database)
    {
        _logger = logger;
        _database = database;
    }

    [HttpGet(Name = "GetAllStudents")]
    public IEnumerable<Student> GetAllStudents()
    {
        _logger.LogInformation("/Students GET request received");
        return _database.GetStudents();
    }

    [HttpGet("{id}", Name = "GetStudent")]
    public IActionResult GetStudent(string id)
    {
        _logger.LogInformation($"/Students/{id} GET request received");
        var student = _database.GetStudent(id);
        
        if (student == null)
        {
            return NotFound($"Student with id {id} not found");
        }

        return Ok(student);
    }
    
    [HttpPost(Name = "CreateStudent")]
    public async Task<IActionResult> CreateStudent(Student? student)
    {
        _logger.LogInformation("/Students POST request received with body: " + student);
        if(!ValidStudent(student))
        {
            return BadRequest("Invalid student data");
        }
        if(_database.GetStudent(student.IndexNumber) != null)
        {
            return BadRequest("Student with index number " + student.IndexNumber + " already exists");
        }
        var res = await _database.CreateStudent(student);
        return CreatedAtAction("CreateStudent", new { id = student.IndexNumber }, res);
    }

    [HttpPut("{id}", Name = "UpdateStudent")]
    public async Task<IActionResult> UpdateStudent(string id, Student? student)
    {
        _logger.LogInformation($"/Students/{id} PUT request received with body: ${student}");
        if(!ValidStudent(student))
        {
            return BadRequest("Invalid student data");
        }
        if(_database.GetStudent(id) == null)
        {
            return BadRequest("Student with index number " + student.IndexNumber + " does not exists");
        }
        var updated = await _database.UpdateStudent(id, student);
        return Ok(updated);
    }
    
    [HttpDelete("{id}", Name = "DeleteStudent")]
    public async Task<IActionResult> DeleteStudent(string id)
    {
        _logger.LogInformation($"/Students/{id} DELETE request received");
        var removed = await _database.DeleteStudent(id);
        return Ok(removed);
    }
    
    private bool ValidStudent(Student? student)
    {
        if (student == null)
        {
            return false;
        }
        if (string.IsNullOrEmpty(student.FirstName))
        {
            return false;
        }
        if (string.IsNullOrEmpty(student.Surname))
        {
            return false;
        }
        if (string.IsNullOrEmpty(student.IndexNumber))
        {
            return false;
        }
        if (string.IsNullOrEmpty(student.BirthDate))
        {
            return false;
        }
        if (string.IsNullOrEmpty(student.StudiesName))
        {
            return false;
        }
        if (string.IsNullOrEmpty(student.StudiesMode))
        {
            return false;
        }
        if (string.IsNullOrEmpty(student.Email))
        {
            return false;
        }
        if (string.IsNullOrEmpty(student.FathersName))
        {
            return false;
        }
        if (string.IsNullOrEmpty(student.MothersName))
        {
            return false;
        }
        return true;
    }
}