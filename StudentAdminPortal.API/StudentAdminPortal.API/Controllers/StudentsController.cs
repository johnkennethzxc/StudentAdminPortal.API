using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DataModels = StudentAdminPortal.API.DataModels;
using StudentAdminPortal.API.DomainModels;
using StudentAdminPortal.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace StudentAdminPortal.API.Controllers
{
    [ApiController]
    public class StudentsController : Controller
    {
        private readonly IStudentRepository studentRepository;
        private readonly IMapper mapper;
        private readonly IImageRepository imageRepository;

        public StudentsController(IStudentRepository studentRepository, IMapper mapper,
            IImageRepository imageRepository)
        {
            this.studentRepository = studentRepository;
            this.mapper = mapper;
            this.imageRepository = imageRepository;
        }


        [HttpGet]
        [Route("[controller]")]
        public async Task<IActionResult> GetAllStudentsAsync()
        {
            //return Ok(studentRepository.GetStudents());
            var students = await studentRepository.GetStudentsAsync();

            //var domainModelStudents = new List<Student>();

            //foreach (var student in students)
            //{
            //    domainModelStudents.Add(new Student()
            //    {
            //        Id = student.Id,
            //        FirstName = student.FirstName,
            //        LastName = student.LastName,
            //        DateOfBirth = student.DateOfBirth,
            //        Email = student.Email,
            //        Mobile = student.Mobile,
            //        ProfileImageUrl = student.ProfileImageUrl,
            //        GenderId = student.GenderId,
            //        Address = new Address()
            //        {
            //            Id = student.Address.Id,
            //            PhysicalAddress = student.Address.PhysicalAddress,
            //            PostalAddress = student.Address.PostalAddress
            //        },
            //        Gender = new Gender()
            //        { 
            //            Id = student.Gender.Id,
            //            Description = student.Gender.Description
            //        }
            //    });
            //}

            // This is using AutoMapper
            return Ok(mapper.Map<List<Student>>(students));


            //return Ok(domainModelStudents);
        }

        [HttpGet]
        [Route("[controller]/{studentId:guid}"), ActionName("GetStudentAsync")]
        public async Task<IActionResult> GetStudentAsync([FromRoute] Guid studentId)
        {
            var student = await studentRepository.GetStudentAsync(studentId);

            if (student == null) return NotFound();

            return Ok(mapper.Map<Student>(student));
        }

        [HttpPut]
        [Route("[controller]/{studentId:guid}")]
        public async Task<IActionResult> UpdateStudentAsync([FromRoute] Guid studentId, [FromBody] UpdateStudentRequest request)
        {
            if (await studentRepository.Exists(studentId))
            {
                var updatedStudent = await studentRepository
                    .UpdateStudent(studentId, mapper.Map<DataModels.Student>(request));

                if (updatedStudent != null)
                {
                    return Ok(mapper.Map<Student>(updatedStudent));
                }
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("[controller]/{studentId:guid}")]
        public async Task<IActionResult> DeleteStudentAsync([FromRoute] Guid studentId)
        {
            if (await studentRepository.Exists(studentId))
            {
                var student = await studentRepository.DeleteStudent(studentId);
                return Ok(mapper.Map<Student>(student));
            }

            return NotFound();
        }

        [HttpPost]
        [Route("[controller]/Add")]
        public async Task<IActionResult> AddStudentAsync([FromBody] AddStudentRequest request)
        {
            var student = await studentRepository.AddStudent(mapper.Map<DataModels.Student>(request));
            return CreatedAtAction(nameof(GetStudentAsync), new { studentId = student.Id }, 
                mapper.Map<Student>(student));
        }

        [HttpPost]
        [Route("[controller]/{studentId:guid}/upload-image")]
        public async Task<IActionResult> UploadProfileImage([FromRoute] Guid studentId, IFormFile profileImage)
        {
            if (await studentRepository.Exists(studentId))
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(profileImage.FileName);
                var fileImagePath = await imageRepository.Upload(profileImage, fileName);
                if (await studentRepository.UpdateProfileImage(studentId, fileImagePath))
                {
                    return Ok(fileImagePath);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading image");
            }

            return NotFound();
        }
    }
}
