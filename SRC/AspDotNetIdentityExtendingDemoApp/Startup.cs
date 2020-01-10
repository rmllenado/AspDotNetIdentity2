using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using AspDotNetIdentityOwinDemoApp.Models;

[assembly: OwinStartup(typeof(AspDotNetIdentityOwinDemoApp.Startup))]

namespace AspDotNetIdentityOwinDemoApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            const string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Database=Pluralsight.AspNetIdentityDemo.Module3.1;trusted_connection=true";
            app.CreatePerOwinContext(() => new ExtendedUserDbContext(connectionString));
            app.CreatePerOwinContext<UserStore<ExtendedUser>>((opt, cont) => new UserStore<ExtendedUser>(cont.Get<ExtendedUserDbContext>()));
            app.CreatePerOwinContext<UserManager<ExtendedUser>>((opt, cont) => new UserManager<ExtendedUser>(cont.Get<UserStore<ExtendedUser>>()));

            app.CreatePerOwinContext<SignInManager<ExtendedUser, string>>((opt, cont) => new SignInManager<ExtendedUser, string>(cont.Get<UserManager<ExtendedUser>>(), cont.Authentication));
            app.UseCookieAuthentication(new CookieAuthenticationOptions() { AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie });
        }
    }
}
