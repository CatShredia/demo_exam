using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoExam1.Data.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<RepairTask> Tasks { get; set; }
    }
}
