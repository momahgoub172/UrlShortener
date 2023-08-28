
using UrlShortener.DTOs;
using UrlShortener.Models;

namespace UrlShortener.Contracts
{
    public interface IUrlRepository
    {
        public  Task<string> GetShortUrlUingLongUrlAsync(string Longurl);
        public  Task<bool> ShortenUrlAsync(Url request);
        public Task<bool> DeleteUrlAsync(string ShortUrl);
    }
}
