using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrack.Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Email { get; set; }
    }
}
