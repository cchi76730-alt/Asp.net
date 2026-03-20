using System.ComponentModel.DataAnnotations;

namespace Demo01.Models
{
    public class Station
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}