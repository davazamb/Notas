using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Notas.Models
{
    public class NotasContext : DbContext
    {
        public NotasContext() : base("DefaultConnection")
        { 

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

        }

        public System.Data.Entity.DbSet<Notas.Models.User> Users { get; set; }
    }
}