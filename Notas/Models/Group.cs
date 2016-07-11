using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Notas.Models
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(100, ErrorMessage = "The field {0} can contain maximun {1} and minimum {2} characters", MinimumLength = 3)]
        [Index("GroupDescriptionIndex", IsUnique = true)]
        public string Description { get; set; }
        [Display(Name = "Profesor")]
        [Required(ErrorMessage = "The field {0} is required")]
        public int UserId { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
        [JsonIgnore]
        public virtual ICollection<GroupDetail> GroupDetails { get; set; }
    }
}