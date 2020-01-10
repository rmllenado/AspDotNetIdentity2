using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace AspDotNetIdentityDemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Module1();

            Module2("llenado@ymcaret.org", "tange123");
            Module2("tange@ymcaret.org", "tange123");

            Console.ReadLine();
        }

        static void Module1()
        {
            var userStore = new UserStore<IdentityUser>();
            var userManager = new UserManager<IdentityUser>(userStore);

            // Create IdentityUser
            var creationResult = userManager.Create(new IdentityUser("llenado@ymcaret.org"), "tange123");

            var user = userManager.FindByName("llenado@ymcaret.org");

            // Create claim
            var claimResult = userManager.AddClaim(user.Id, new Claim("first_name", "ramon"));

            // Remove claim
            //var claimResult2 = userManager.RemoveClaim(user.Id, new Claim("first_name", "ramon"));

            // Check password
            var checkPasswordResult = userManager.CheckPassword(user, "tange123");
            Console.WriteLine("Password Match: {0}", checkPasswordResult);
        }

        static void Module2(string username, string password)
        {
            var userStore = new CustomUserStore(new CustomUserDbContext());
            var userManager = new UserManager<CustomUser, int>(userStore);

            var creationResult = userManager.Create(new CustomUser { UserName = username }, password);

            var user = userManager.FindByName(username);

            // Check password
            var checkPasswordResult = userManager.CheckPassword(user, password);
            Console.WriteLine("Password Match: {0}", checkPasswordResult);
        }
    }

    public class CustomUser : IUser<int>
    {
        public int Id { get; set; }
        public  string UserName{ get; set; }
        public string PasswordHash { get; set; }
    }

    public class CustomUserDbContext : DbContext
    {
        public CustomUserDbContext() : base("DefaultConnection2") { }

        public DbSet<CustomUser> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var user = modelBuilder.Entity<CustomUser>();
            user.ToTable("Users");
            user.HasKey(x => x.Id);
            user.Property(x => x.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            user.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = true }));

            base.OnModelCreating(modelBuilder);
        }
    }

    public class CustomUserStore : IUserPasswordStore<CustomUser, int>
    {
        private readonly CustomUserDbContext context;
        public CustomUserStore(CustomUserDbContext context)
        {
            this.context = context;
        }
        public Task<string> GetPasswordHashAsync(CustomUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(CustomUser user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetPasswordHashAsync(CustomUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task CreateAsync(CustomUser user)
        {
            context.Users.Add(user);
            return context.SaveChangesAsync();
        }

        public Task DeleteAsync(CustomUser user)
        {
            context.Users.Remove(user);
            return context.SaveChangesAsync();
        }

        public Task<CustomUser> FindByIdAsync(int userId)
        {
            return context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }

        public Task<CustomUser> FindByNameAsync(string userName)
        {
            return context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        }

        public Task UpdateAsync(CustomUser user)
        {
            context.Users.Attach(user);
            return context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
