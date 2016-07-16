using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Notas.Models
{
    public class Note
    {
        [Key]
        public int NoteId { get; set; }
        [Required]
        public int GroupDetailId { get; set; }

        [Range(0, 5, ErrorMessage = "Nota Invalida")]
        public float Percentage { get; set; }
        [Range(0,5,ErrorMessage = "Nota Invalida")]
        public float Qualification { get; set; }

        [JsonIgnore]
        public virtual GroupDetail GroupDetail { get; set; }

    }
}