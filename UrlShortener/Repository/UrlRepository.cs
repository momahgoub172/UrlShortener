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

            string query = @"INSERT INTO UrlMappings (LongUrl, ShortUrl, CreationDate, IsActive) VALUES (@LongUrl, @ShortUrl, @CreationDate, @IsActive);";
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
    }
}
