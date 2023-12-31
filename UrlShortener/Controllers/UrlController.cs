﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;
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

        public UrlController(IUrlRepository urlRepository, DapperContext dapperContext)
        {
            _urlRepository = urlRepository;
            _dapperContext = dapperContext;
        }

        [HttpPost("ShortenUrl")]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUrl = await _urlRepository.GetShortUrlUingLongUrlAsync(request.LongUrl);
            if (existingUrl != null)
            {
                return Ok(existingUrl);
            }

            string shortCode = UrlHelper.GenerateUniqueShortCode(_dapperContext);
            string shortUrl = GetShortUrl(shortCode);
            

            var url = new Url
            {
                LongUrl = request.LongUrl,
                ShortUrl = shortUrl,
                CreationDate = DateTime.Now,
                IsActive = true,
                ShortCode=shortCode
            };

            var added = await _urlRepository.ShortenUrlAsync(url);
            if (added == true)
            {
                return Ok(new ShortenedUrlResponse
                {
                    ShortUrl =shortUrl
                });
            }
            return BadRequest("something wrong");
        }

        [HttpDelete("DeleteUrl")]
        public async Task<IActionResult> DeleteUrl(string shorturl)
        {
            var deleted = await _urlRepository.DeleteUrlAsync(shorturl);
            if (deleted == true)
            {
                return Ok("Deleted");
            }
            return BadRequest("Not Deleted");
        }


        [HttpGet("qrcode/{shortUrl}")]
        public IActionResult GenerateQRCode(string shortUrl)
        {
            //Generate QR Code
            QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(shortUrl, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            //Convert the QR code to a Bitmap
            Bitmap qrCodeImage = qrCode.GetGraphic(10);

            // Convert the Bitmap to a byte array
            byte[] imageBytes;
            using (MemoryStream stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                imageBytes = stream.ToArray();
            }
            return File(imageBytes, "image/png");
        }


        [Route("/{shortCode}")]
        [HttpGet]
        public async Task<IActionResult> RedirectUrl(string shortCode)
        {
            var url = await _urlRepository.GetLongUrlUsingShortUrlAsync(shortCode);
            if(url == null)
                return NotFound();
            if (url.ExpirationDate.HasValue && url.ExpirationDate < DateTime.UtcNow)
                return BadRequest("Expired");
            if(!url.IsActive)
                return BadRequest("Deactivated");

            return Redirect(url.LongUrl);
        }

        [HttpPut("activate")]
        public async Task<IActionResult> ActivateUrl(string shorturl)
        {
            var Activated=await _urlRepository.ActivateUrl(shorturl);
            if (Activated == true)
                return Ok("Activated");
            return BadRequest("Not Activated");
        }

        [HttpPut("deactivate")]
        public async Task<IActionResult> DeactivateUrl(string shorturl)
        {
            var Activated = await _urlRepository.DeactivateUrl(shorturl);
            if (Activated == true)
                return Ok("Deactivated");
            return BadRequest("Not Deactivated");
        }


        [HttpGet("GetAllActivatedUrls")]
        public async Task<IEnumerable<Url>> GetAllActivatedUrls()
        {
          return  await _urlRepository.GetAllActivatedUrls();
        }

        [HttpGet("GetAllDeactivatedUrls")]
        public async Task<IEnumerable<Url>> GetAllDeactivatedUrls()
        {
            return await _urlRepository.GetAllDeactivatedUrls();
        }


        [HttpPut("UpdateExpiratinDate")]
        public async Task<IActionResult> UpdateExpiratinDate(string shorturl , DateTime NewDate)
        {
            var updated = await _urlRepository.UpdateExpiratinDate(shorturl, NewDate);
            if (updated == true)
                return Ok("updated");
            return BadRequest("Not updated");
        }


        private string GetShortUrl(string shortCode)
        {
            return $"http://localhost:5262/{shortCode}";
        }
    }
}
