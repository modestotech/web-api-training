using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SendTestJWT
{
    class Program
    {
        static void Main(string[] args)
        {
            // Service URL to call
            string _url = "http://localhost:64827/auth";

            // The tokes isser urn, and the audience urn I intend this token for
            string _issuer = "http://my.tokenissuer.com";
            string _audience = "https://my.company.com";

            // Load the private key certificate from the local file
            string path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            var directory = System.IO.Path.GetDirectoryName(path);
            var certFilePre = directory + "\\Certificate\\CourseCert.pfx";
            var certFile = "\\" + certFilePre.Substring(5);
            var certificate = new X509Certificate2();
            certificate.Import(certFile, "abc12345", X509KeyStorageFlags.PersistKeySet);

            // Create a digital signing key from the secret key in the certificate
            var signingCredentials = new X509SecurityKey(certificate);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, "TestUserName"),
                new Claim(ClaimTypes.Email, "testuser@mycompany.com"),
                new Claim("MyCustomClaim", "My special value")
            };

            // Define the token elements
            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Audience = _audience,
                Issuer = _issuer,
                Subject = new ClaimsIdentity(claims, "Bearer"),
                SigningCredentials = new SigningCredentials(signingCredentials,
                                        SecurityAlgorithms.RsaSha256Signature),
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow.AddMinutes(-1),
                Expires = DateTime.UtcNow.AddHours(12)
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(securityTokenDescriptor);
            var tokenString = handler.WriteToken(token);

            Console.WriteLine(tokenString);

            // Make a REST call with the JWT string
            var task = Task.Run(async () => await GetValuesAsync(_url, tokenString));
            var values = task.Result;

            foreach (var s in values)
                Console.WriteLine(s);

            Console.ReadKey();
        }

        static async Task<List<string>> GetValuesAsync(string path, string token)
        {
            List<string> values = new List<string>();
            var client = new HttpClient();

            client.BaseAddress = new Uri(path);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer", token);

            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
                values = await response.Content.ReadAsAsync<List<string>>();
            else
                values.Add(response.ToString());

            return values;
        }
    }
}
