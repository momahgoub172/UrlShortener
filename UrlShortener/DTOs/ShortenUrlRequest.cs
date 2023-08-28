namespace UrlShortener.DTOs
{
    public class ShortenUrlRequest
    {
        public string LongUrl { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
