﻿using System.ComponentModel.DataAnnotations;

namespace TechSavvy.Models.ViewModels
{
    public class ProductDetailsViewModel
    {
        public ProductModel ProductDetails { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập bình luận sản phẩm")]
        public string Comment { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Yêu cầu chọn số sao")]
        [Range(1, 5, ErrorMessage = "Chỉ được chọn từ 1 đến 5 sao")]
        public int Star { get; set; }
        public List<RatingModel> Reviews { get; set; }
    }
}
