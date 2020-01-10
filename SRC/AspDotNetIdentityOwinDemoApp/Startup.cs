using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using System.Configuration;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Owin.Security.Google;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(AspDotNetIdentityOwinDemoApp.Startup))]

namespace AspDotNetIdentityOwinDemoApp
{
    public class Startup
    {
        //public void Configuration(IAppBuilder app)
        //{
        //    // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        //    const string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Database=Pluralsight.AspNetIdentityDemo.Module2.2;trusted_connection=true";
        //    app.CreatePerOwinContext(() => new IdentityDbContext(connectionString));
        //    app.CreatePerOwinContext<UserStore<IdentityUser>>((opt, cont) => new UserStore<IdentityUser>(cont.Get<IdentityDbContext>()));
        //    app.CreatePerOwinContext<UserManager<IdentityUser>>((opt, cont) =>
        //        {
        //            var usermanager = new UserManager<IdentityUser>(cont.Get<UserStore<IdentityUser>>());

        //            usermanager.RegisterTwoFactorProvider("SMS", new PhoneNumberTokenProvider<IdentityUser> { MessageFormat = "Token: {0}" });
        //            usermanager.SmsService = new SmsService();

        //            usermanager.UserTokenProvider = new DataProtectorTokenProvider<IdentityUser>(opt.DataProtectionProvider.Create());
        //            usermanager.EmailService = new EmailService();

        //            usermanager.UserValidator = new UserValidator<IdentityUser>(usermanager) { RequireUniqueEmail = true };
        //            usermanager.PasswordValidator = new PasswordValidator
        //            {
        //                RequireDigit = true,
        //                RequireLowercase = true,
        //                RequireNonLetterOrDigit = true,
        //                RequireUppercase = true,
        //                RequiredLength = 8
        //            };

        //            usermanager.UserLockoutEnabledByDefault = true;
        //            usermanager.MaxFailedAccessAttemptsBeforeLockout = 2;
        //            usermanager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(3);

        //            return usermanager;
        //        });

        //    app.CreatePerOwinContext<SignInManager<IdentityUser, string>>((opt, cont) => new SignInManager<IdentityUser, string>(cont.Get<UserManager<IdentityUser>>(), cont.Authentication));

        //    app.UseCookieAuthentication(new CookieAuthenticationOptions()
        //    {
        //        AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
        //        Provider = new CookieAuthenticationProvider
        //        {
        //            OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UserManager<IdentityUser>, IdentityUser>(validateInterval: TimeSpan.FromSeconds(3)
        //            ,regenerateIdentity: (manager, user) => manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie))
        //        }
        //    });

        //    app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
        //    app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

        //    app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

        //    app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
        //    {
        //        ClientId = ConfigurationManager.AppSettings["google:ClientId"],
        //        ClientSecret = ConfigurationManager.AppSettings["google:ClientSecret"],
        //        Caption = "Google"
        //    });
        //}

        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<UserManager<IdentityUser, string>>(
                () => DependencyResolver.Current.GetService<UserManager<IdentityUser, string>>());

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UserManager<IdentityUser, string>, IdentityUser>(validateInterval: TimeSpan.FromSeconds(3)
                    , regenerateIdentity: (manager, user) => manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie))
                }
            });

            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["google:ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["google:ClientSecret"],
                Caption = "Google"
            });
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            var sid = ConfigurationManager.AppSettings["twilio:Sid"];
            var token = ConfigurationManager.AppSettings["twilio:Token"];
            var from = ConfigurationManager.AppSettings["twilio:From"];

            TwilioClient.Init(sid, token);
            await MessageResource.CreateAsync(new PhoneNumber(message.Destination), from: new PhoneNumber(from), body: message.Body);
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            using (var client = new System.Net.Mail.SmtpClient("mail01.ymcaret.org"))
            using (var email = new System.Net.Mail.MailMessage())
            {
                email.From = new System.Net.Mail.MailAddress("noreply@ymcaret.org");
                //email.To.Add(message.Destination);
                email.To.Add("llenado@ymcaret.org");
                email.Subject = message.Subject;
                email.Body = message.Body;

                await Task.Run(() => { client.Send(email); });
            }
        }
    }
}
