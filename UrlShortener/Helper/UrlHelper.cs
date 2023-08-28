using Dapper;
using UrlShortener.Data;

namespace UrlShortener.Helper
{
    public static class UrlHelper
    {
        public static string GenerateUniqueShortCode(DapperContext context)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            string code;
            string existingCode;

            // Generate a short code and check if it's already in use
            do
            {
                code = new string(Enumerable.Repeat(chars, 8).Select(c => c[random.Next(c.Length)]).ToArray());


                //using dapper
                var connection = context.CreateConnection();
                 existingCode = connection.ExecuteScalar<string>("Select ShortCode from UrlMappings where ShortCode = @shortcode", new { ShortCode = code });
            }
            while (!string.IsNullOrEmpty(existingCode));
            return code;
        }
    }
}
