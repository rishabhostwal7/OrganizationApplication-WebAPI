using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class EmployeeDB
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public DateTime Date_Of_Joining { get; set; }
        public string Photo_File_Name { get; set; }
    }
}
