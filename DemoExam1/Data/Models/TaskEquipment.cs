namespace DemoExam1.Data.Models
{
    public class TaskEquipment
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public RepairTask Task { get; set; } = null!;
        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; } = null!;
        public string Problem { get; set; } = string.Empty;
        public string TypeOfProblem { get; set; } = string.Empty;
    }
}
