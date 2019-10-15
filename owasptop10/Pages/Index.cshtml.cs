using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using owasptop10.Models.DB;

namespace owasptop10.Pages
{
    public class IndexModel : PageModel
    {

        private readonly Owasptop10Context _dbcontext;

        public IndexModel(Owasptop10Context dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public IActionResult OnGet()
        {
            try
            {
                // Verification.  
                if (this.User.Identity.IsAuthenticated)
                {
                    // Home Page.  
                    return this.RedirectToPage("/Home/Index");
                }
            }
            catch (Exception ex)
            {
                // Info  
                Console.Write(ex);
            }

            // Info.  
            return this.Page();
        }

        [BindProperty]
        public LoginViewModel LoginModel { get; set; }


        public async Task<IActionResult> OnPostLogIn()
        {
            try
            {
                // Verification.  
                if (ModelState.IsValid)
                {
                    var userName = this.LoginModel.UserName;
                    var base64Salt = _dbcontext.Users
                        .FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))?.PasswordSalt;
                    if (!string.IsNullOrWhiteSpace(base64Salt))
                    {
                        var salt = Convert.FromBase64String(base64Salt);
                        string usrPwdHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                password: this.LoginModel.Password,
                                salt: salt,
                                prf: KeyDerivationPrf.HMACSHA1,
                                iterationCount: 10000,
                                numBytesRequested: 256 / 8));
                        //https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-3.0
                        //https://github.com/aspnet/AspNetCore/blob/master/src/Identity/Extensions.Core/src/PasswordHasher.cs

                        var isExist = await _dbcontext.LoginByUsernamePasswordAsync(userName, usrPwdHash);
                        if (isExist)
                        {
                            // Login In.  
                            await this.SignInUser(userName, false);

                            // Info.  
                            return this.RedirectToPage("/Home/Index");
                        }
                    }                     
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                // Info  
                Console.Write(ex);
            }

            // Info.  
            return this.Page();
        }

        private async Task SignInUser(string userName, bool isPersistent)
        {
            // Initialization.  
            var claims = new List<Claim>();

            try
            {
                // Setting  
                claims.Add(new Claim(ClaimTypes.Name, userName));
                var claimIdenties = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimPrincipal = new ClaimsPrincipal(claimIdenties);
                var authenticationManager = Request.HttpContext;
                // Sign In.  
                await authenticationManager.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, new AuthenticationProperties() { IsPersistent = isPersistent });
            }
            catch (Exception ex)
            {
                // Info  
                throw ex;
            }
        }
    }
}
