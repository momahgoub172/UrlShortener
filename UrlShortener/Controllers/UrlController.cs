using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Contracts;
using UrlShortener.Data;
using UrlShortener.DTOs;
using UrlShortener.Helper;
using UrlShortener.Models;

namespace UrlShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly IUrlRepository _urlRepository;
        private readonly DapperContext _dapperContext;

        public UrlController(IUrlRepository urlRepository,DapperContext dapperContext)
        {
            _urlRepository = urlRepository;
            _dapperContext = dapperContext;
        }

        [HttpPost("ShortenUrl")]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUrl = await _urlRepository.GetShortUrlUingLongUrlAsync(request.LongUrl);
            if (existingUrl != null)
            {
                return Ok(GetShortUrl(existingUrl));
            }

            string shortCode = UrlHelper.GenerateUniqueShortCode(_dapperContext);

            var url = new Url
            {
                LongUrl= request.LongUrl,
                ShortCode= shortCode,
                CreationDate=DateTime.Now,
                IsActive=true
            };

            var added =await _urlRepository.ShortenUrlAsync(url);
            if (added == true)
            {
                return Ok(new ShortenedUrlResponse
                {
                    ShortUrl = GetShortUrl(shortCode)
                });
            }
            return BadRequest("something wrong");
        }


        private string GetShortUrl(string shortCode)
        {
            return $"http://localhost/{shortCode}";
        }
    }
}
