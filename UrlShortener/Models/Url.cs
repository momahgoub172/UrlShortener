﻿using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Models
{
    public class Url
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(int.MaxValue)] // Adjust the length as needed
        public string LongUrl { get; set; }

        [Required]
        [MaxLength(100)] // Adjust the length as needed
        public string ShortUrl { get; set; }

        [MaxLength(8)]
        public string ShortCode { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpirationDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
