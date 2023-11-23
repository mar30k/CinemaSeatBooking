﻿namespace CinemaSeatBooking.Models
{
    public class MovieModel
    {
        public string? MovieCode { get; set; }
        public string? ArticleCode { get; set; }
        public string? MovieName { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyCode { get; set; }
        public string? BranchName { get; set; }
        public string? BranchCode { get; set; }
        public string? MoviePreviewImageUrl { get; set; }
        public int? ReleaseYear { get; set; }
        public string? CompanyTinNumber { get; set; }
        public string? PosterUrl {  get; set; }
    }
}