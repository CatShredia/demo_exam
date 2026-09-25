using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoExam1.Data.Models
{
    public class OrderSpare
    {
        public int Id { get; set; }
        public string NameSpare { get; set; } = string.Empty;
        public decimal ExpectedCost { get; set; } = 0;
        public decimal? TotalCost { get; set; }

        public int TaskId { get; set; }
        public RepairTask Task { get; set; }

        public DateTime DateOfOrder{ get; set; } = DateTime.UtcNow;
        public DateTime? DateOfGetting{ get; set; }

    }
}
