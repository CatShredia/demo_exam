using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoExam1.Data.Models
{
    public enum UserRole { Admin, Manager, Specialist};
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public ICollection<RepairTask> Tasks { get; set; }
    }
}
