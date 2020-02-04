using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PruebaFront.Models.Login;

namespace PruebaFront.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private string APIUrl = string.Empty;
        public LoginController(IConfiguration iConfig,IHttpContextAccessor httpContextAccessor)
        {
            _config = iConfig;
            _httpContextAccessor = httpContextAccessor;
            APIUrl = _config.GetValue<string>("APIUrl");
        }
        public IActionResult Index()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel User)
        {
            //We will make a GET request to a really cool website...
            if (!ModelState.IsValid)
            {

            }
            string baseUrl = APIUrl + "api/Access/GetToken";
            //The 'using' will help to prevent memory leaks.
            //Create a new instance of HttpClient

            //Dictionary<string, string> UserDto = new Dictionary<string, string>();
            var myContent = JsonConvert.SerializeObject(User);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");



            HttpClient client = new HttpClient();

            //Setting up the response...         

            //using (HttpResponseMessage res = await client.PostAsync(baseUrl))
            var result = client.PostAsync(baseUrl, byteContent).Result;
            using (HttpContent content = result.Content /*res.Content*/)
            {

                string data = await content.ReadAsStringAsync();
                if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ModelState.AddModelError("CustomError", "Usuario no existe.");
                    return View("Index");
                }

                _httpContextAccessor.HttpContext.Session.SetString("JWToken", data);

                var jwt = data;
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                IEnumerable<Claim> claims = token.Claims;
                var userNameIdentifier = (claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()).Value;
                var userNameClaim = (claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault()).Value;
                var name = (claims.Where(x => x.Type == ClaimTypes.Name).FirstOrDefault()).Value;
                var surname = (claims.Where(x => x.Type == ClaimTypes.Surname).FirstOrDefault()).Value;


                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim("access_token", data));
                identity.AddClaim(new Claim("NameIdentifier", userNameClaim));
                identity.AddClaim(new Claim("Email", userNameClaim));

                identity.AddClaim(new Claim("Surname", surname));


                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            }
            return RedirectToAction("Index", "Receipt");

        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }
    }
}