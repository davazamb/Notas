using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Notas.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Display(Name = "E-Mail")]
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(100, ErrorMessage = "The field {0} can contain maximun {1} and minimum {2} characters", MinimumLength = 7)]
        [DataType(DataType.EmailAddress)]
        [Index("UserNameIndex", IsUnique = true)]
        public string UserName { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(50, ErrorMessage = "The field {0} can contain maximun {1} and minimum {2} characters", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Display(Name = "Apellido")]
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(50, ErrorMessage = "The field {0} can contain maximun {1} and minimum {2} characters", MinimumLength = 2)]
        public string LastName { get; set; }

        [Display(Name = "Usuario")]
        public string FullName { get { return string.Format("{0} {1}", this.FirstName, this.LastName); } }

        [Display(Name = "Telefono")]
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(20, ErrorMessage = "The field {0} can contain maximun {1} and minimum {2} characters", MinimumLength = 7)]
        public string Phone { get; set; }

        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(100, ErrorMessage = "The field {0} can contain maximun {1} and minimum {2} characters", MinimumLength = 10)]
        public string Address { get; set; }

        [Display(Name = "Foto")]
        [DataType(DataType.ImageUrl)]
        public string Photo { get; set; }

        [Display(Name = "Estudiante")]
        public bool IsStudent { get; set; }

        [Display(Name = "Profesor")]
        public bool IsTeacher { get; set; }

        [JsonIgnore]
        public virtual ICollection<Group> Groups { get; set; }
        [JsonIgnore]
        public virtual ICollection<GroupDetail> GroupDetails { get; set; }

    }
}