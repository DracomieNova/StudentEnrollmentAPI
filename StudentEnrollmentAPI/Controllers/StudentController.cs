﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentApi.Data;
using StudentEnrollmentApi.Models;

namespace StudentEnrollmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {

        private readonly StudentDbContext _context;

        public StudentController(StudentDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            var content = _context.Students.Include(b => b.Parish).Include(b => b.Size).Include(b => b.Course).ToList();

            if (content == null)
            {
                return BadRequest();
            }
            return Ok(content);
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentById(int id)
        {
            var content = _context.Students.FirstOrDefault(b => b.Id == id);
            if (content == null)
            {
                return BadRequest();
            }
            return Ok(content);
        }

        [HttpPost]
        public IActionResult CreateStudent([FromBody] Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
        }

        [HttpPost]
        [Route("FileUpload")]
        public async Task<IActionResult> CreateStudentWithFile([FromForm] studentDTO studentDTO)
        {
            if (ModelState.IsValid)
            {
                var studentImageFile = studentDTO.StudntIdImageFile;

                if (studentImageFile != null && studentImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid() + "-" + studentImageFile.FileName;

                    var apiFilePath = Path.Combine("api", "server", "uploads", fileName);

                    using (var stream = new FileStream(apiFilePath, FileMode.Create))
                    {
                        await studentImageFile.CopyToAsync(stream);
                    }

                    Student student = new()
                    {
                        StudentName = studentDTO.StudentName,
                        EmailAddress = studentDTO.EmailAddress,
                        PhoneNumber = studentDTO.PhoneNumber,
                        ProgramId = studentDTO.ProgramId,
                        ParishId = studentDTO.ParishId,
                        SizeId = studentDTO.SizeId,
                        StudentImageFilePath = apiFilePath != String.Empty ? apiFilePath : ""
                    };

                    _context.Students.Add(student);
                    _context.SaveChanges();

                    return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpGet("files/{filename}")]

        public IActionResult GetFile(string filename)
        {
            string filePath = Path.Combine("api", "server", "uploads", filename);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            string contentType = GetContentType(filePath);

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            return new FileStreamResult(fileStream, contentType);
        }

        private string GetContentType(string filename)
        {
            string ext = Path.GetExtension(filename).ToLowerInvariant();

            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".pdf":
                    return "application/pdf";
                default:
                    return "application/octet-stream";
            }
        }

        [HttpPut("{id}")]

        public IActionResult UpdateStudent(int id, [FromBody] Student student)
        {
            var content = _context.Students.FirstOrDefault(x => x.Id == id);

            if (content == null)
            {
                return NotFound();
            }

            _context.Students.Update(student);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            if (_context.Students == null)
            {
                return BadRequest("Entity set 'Students' is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return Ok(student);
        }


        [HttpGet]
        [Route("course")]
        public IActionResult GetCourse()
        {
            var content = _context.Courses.ToList();

            if (content == null)
            {
                return BadRequest();
            }

            return Ok(content);
        }

        [HttpGet]
        [Route("parish")]
        public IActionResult GetParish()
        {
            var content = _context.Parishes.ToList();

            if (content == null)
            {
                return BadRequest();
            }

            return Ok(content);
        }

        [HttpGet]
        [Route("size")]
        public IActionResult GetSize()
        {
            var content = _context.Sizes.ToList();

            if (content == null)
            {
                return BadRequest();
            }

            return Ok(content);
        }
    }



}
