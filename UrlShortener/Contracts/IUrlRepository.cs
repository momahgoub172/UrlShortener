
using UrlShortener.DTOs;
using UrlShortener.Models;

namespace UrlShortener.Contracts
{
    public interface IUrlRepository
    {
        public  Task<string> GetShortUrlUingLongUrlAsync(string Longurl);
        public Task<Url> GetLongUrlUsingShortUrlAsync(string shorturl);
        public  Task<bool> ShortenUrlAsync(Url request);
        public Task<bool> DeleteUrlAsync(string ShortUrl);
        public Task<bool> ActivateUrl(string ShortUrl);
        public Task<bool> DeactivateUrl(string ShortUrl);
        public Task<IEnumerable<Url>> GetAllActivatedUrls();
        public Task<IEnumerable<Url>> GetAllDeactivatedUrls();
        public Task<bool> UpdateExpiratinDate(string ShortUrl, DateTime NewDate);
    }
}
