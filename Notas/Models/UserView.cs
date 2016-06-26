using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notas.Models
{
    public class UserView
    {
        public User User { get; set; }
        public HttpPostedFileBase Photo { get; set; }
    }
}