using System.ComponentModel.DataAnnotations;


namespace Blazor1.Models
{
    public class Agenda
    {
        public int Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [StringLength(500)]
        public string AgendaItem { get; set; } = string.Empty;

        public bool Active { get; set; } = true;

        [Required]
        [StringLength(100)]
        public string InsertBy { get; set; } = string.Empty;

        public DateTime InsertDate { get; set; }

        [StringLength(100)]
        public string? UpdateBy { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}