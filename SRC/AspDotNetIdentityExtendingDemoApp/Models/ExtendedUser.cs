using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Web;

namespace AspDotNetIdentityOwinDemoApp.Models
{
    public class ExtendedUser : IdentityUser
    {
        public ExtendedUser()
        {
            Addresses = new List<Address>();
        }
        public string FullName { get; set; }
        public virtual ICollection<Address> Addresses { get; private set; }
    }

    public class Address
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string AddressLine { get; set; }
        public string Country { get; set; }
        public Address()
        {
            Id = Guid.NewGuid().ToString();
        }
    }

    public class ExtendedUserDbContext : IdentityDbContext<ExtendedUser>
    {
        public ExtendedUserDbContext(string connectionString) : base(connectionString)
        {

        }
        public DbSet<Address> Addresses { get; set; }
        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var address = modelBuilder.Entity<Address>();
            address.ToTable("AspNetUserAddresses");
            address.HasKey(x => x.Id);

            var user = modelBuilder.Entity<ExtendedUser>();
            user.Property(x => x.FullName).IsRequired().HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("FullNameIndex")));
            user.HasMany(x => x.Addresses).WithRequired().HasForeignKey(x => x.UserId);
        }
    }
}