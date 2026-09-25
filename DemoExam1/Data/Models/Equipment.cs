using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoExam1.Data.Models
{
    public class Equipment
    {
        public int Id { get; set; }
        [StringLength(12)]
        public string SerialEquipment { get; set; } = string.Empty;
        public string TypeEquipment { get; set; } = string.Empty;

        public ICollection<RepairTask> Tasks { get; set; }
    }
}
