using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notas.Models
{
    public class MyStudentsResponse
    {
        public double Percentage { get; set; }
        public List<MyStudent> Students { get; set; }
    }
}