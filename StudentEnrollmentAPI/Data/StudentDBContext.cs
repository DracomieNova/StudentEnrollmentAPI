using Microsoft.EntityFrameworkCore;
using StudentEnrollmentApi.Models;

using System.Collections.Generic;

namespace StudentEnrollmentApi.Data
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Parish> Parishes { get; set; }
        public DbSet<Course> Courses { get; set; }


    }       
}   
