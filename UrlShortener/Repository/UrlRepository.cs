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
            return shortUrl.FirstOrDefault()?.ShortCode;
        }

        public async Task<bool> ShortenUrlAsync(Url request)
        {

            string query = @"INSERT INTO UrlMappings (LongUrl, ShortCode, CreationDate, IsActive) VALUES (@LongUrl, @ShortCode, @CreationDate, @IsActive);";
            using var connection = _context.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(query, request);
            if (rowsAffected > 0)
                return true;
            else
                return false;

        }
    }
}
