using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EventAPI.Models
{
    [Table("Events")]
    public class EventInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title of event cannot be empty")]
        [Column(TypeName = "varchar(50)")]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Start date cannot be empty")]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "End date cannot be empty")]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Organizer's name cannot be empty")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Minimum 3 and maximum 50 characters allowed")]
        public string Organizer { get; set; }

        [Required(ErrorMessage = "Location of event cannot be empty")]
        public string Location { get; set; }

        [DataType(DataType.Url)]
        [Required(ErrorMessage = "Registration url cannot be empty")]
        [Display(Name = "Registration Url")]
        public string RegistrationUrl { get; set; }

        [Required(ErrorMessage = "Speaker name cannot be empty")]
        public string Speaker { get; set; }
    }
}
