using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Notas.Models
{
    public class NotasContext : DbContext
    {
        public NotasContext() : base("DefaultConnection")
        { 

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<GroupDetail> GroupDetails { get; set; }
        }
}