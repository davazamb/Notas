using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Notas.Models
{
    public class GroupDetail
    {
        [Key]
        public int GroupDetailId { get; set; }
        [Required]
        public int GroupId { get; set; }
        [Required]
        public int UserId { get; set; }

        [JsonIgnore]
        public virtual Group Group { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
        [JsonIgnore]
        public virtual ICollection<Note> Notes { get; set; }
    }
}