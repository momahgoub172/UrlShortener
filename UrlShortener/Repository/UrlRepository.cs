using UrlShortener.Contracts;
using UrlShortener.Data;
using Dapper;
using UrlShortener.Models;
using UrlShortener.DTOs;

namespace UrlShortener.Repository
{
    public class UrlRepository : IUrlRepository
    {
        private readonly DapperContext _context;

        public UrlRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<string> GetShortUrlUingLongUrlAsync(string Longurl)
        {
            var query = "Select * from UrlMappings where LongUrl = @Longurl";
            using var connection = _context.CreateConnection();
            var shortUrl = await connection.QueryAsync<Url>(query,new {Longurl});
            return shortUrl.FirstOrDefault()?.ShortUrl;
        }

        public async Task<bool> ShortenUrlAsync(Url request)
        {

            string query = @"INSERT INTO UrlMappings (LongUrl, ShortUrl, CreationDate, IsActive, ShortCode) VALUES (@LongUrl, @ShortUrl, @CreationDate, @IsActive , @ShortCode);";
            using var connection = _context.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(query, request);
            if (rowsAffected > 0)
                return true;
            else
                return false;

        }


        public async Task<bool> DeleteUrlAsync(string ShortUrl)
        {
            var query = @"delete from UrlMappings where ShortUrl =@shortUrl";
            using var connection = _context.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(query, new { ShortUrl });
            if (rowsAffected > 0)
                return true;
            else
                return false;
        }

        public async Task<Url> GetLongUrlUsingShortUrlAsync(string shortCode)
        {
            var query = "Select * from UrlMappings where shortCode = @shortCode";
            using var connection = _context.CreateConnection();
            var LongUrl = await connection.QueryAsync<Url>(query, new { shortCode });
            return LongUrl.FirstOrDefault();
        }

        public async Task<bool> ActivateUrl(string ShortUrl)
        {
            var query = "Update UrlMappings set IsActive=1 Where shortUrl = @ShortUrl";
            using var connection = _context.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(query,new { ShortUrl });
            if (rowsAffected > 0)
                return true;
            else
                return false;
        }

        public async Task<bool> DeactivateUrl(string ShortUrl)
        {
            var query = "Update UrlMappings set IsActive=0 Where shortUrl = @ShortUrl";
            using var connection = _context.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(query, new { ShortUrl });
            if (rowsAffected > 0)
                return true;
            else
                return false;
        }

        public async Task<IEnumerable<Url>> GetAllActivatedUrls()
        {
            var query = "select * from UrlMappings where IsActive=1";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Url>(query);
        }

        public async Task<IEnumerable<Url>> GetAllDeactivatedUrls()
        {
            var query = "select * from UrlMappings where IsActive=0";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Url>(query);
        }

        public async Task<bool> UpdateExpiratinDate(string ShortUrl,DateTime NewDate)
        {
            var query = "Update UrlMappings set ExpirationDate=@NewDate Where shortUrl = @ShortUrl";
            using var connection = _context.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(query, new {NewDate ,ShortUrl });
            if (rowsAffected > 0)
                return true;
            else
                return false;
        }

    }
}
