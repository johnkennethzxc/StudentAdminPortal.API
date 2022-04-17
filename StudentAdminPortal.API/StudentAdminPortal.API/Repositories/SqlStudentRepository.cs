using Microsoft.EntityFrameworkCore;
using StudentAdminPortal.API.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAdminPortal.API.Repositories
{
    public class SqlStudentRepository : IStudentRepository
    {
        private readonly StudentAdminContext context;

        public SqlStudentRepository(StudentAdminContext context)
        {
            this.context = context;
        }

        public async Task<List<Student>> GetStudentsAsync()
        {
            //return context.Student
            //    .Include(g => g.Gender)
            //    .Include(a => a.Address)
            //    .ToList();

            return await context.Student
                .Include(nameof(Gender))
                .Include(nameof(Address))
                .ToListAsync();
        }
    }
}
