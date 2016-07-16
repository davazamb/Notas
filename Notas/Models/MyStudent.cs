using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notas.Models
{
    public class MyStudent
    {
        public int GroupDetailId { get; set; }
        public int GroupId { get; set; }
        public User Student { get; set; }

        public double Note { get; set; }
    }
}