using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoExam1.Data.Models
{

    public enum TaskStatus { Registered, Working, Closed, Finished};

    public class RepairTask
    {
        public int Id { get; set; }
        public string Priority { get; set; } = string.Empty;
        public TaskStatus Status{ get; set; }
        public string? SpecialistReport { get; set; } = string.Empty;
        public ICollection<User> Specialists { get; set; }

        public string? ClientComment { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; }
        public string EquipmentProblem { get; set; } = string.Empty;
        public string EquipmentTypeOfProblem { get; set; } = string.Empty;

        public DateTime DateOfRegistration { get; set; } = DateTime.UtcNow;
        public DateTime? DateOfStartWork{ get; set; }
        public DateTime? DateOfClose{ get; set; }

    }
}
