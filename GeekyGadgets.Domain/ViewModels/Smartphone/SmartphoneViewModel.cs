using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GeekyGadgets.Domain.ViewModels.Smartphone
{    
        public class SmartphoneViewModel
        {
            public int Id { get; set; }

            [Display(Name = "Brand")]
            [Required(ErrorMessage = "Brand is required.")]
            public string Brand { get; set; }

            [Display(Name = "Model")]
            [Required(ErrorMessage = "Model is required.")]
            public string Model { get; set; }

            [Display(Name = "Color")]
            [Required(ErrorMessage = "Color is required.")]
            public string Color { get; set; }

            [Display(Name = "Storage")]
            [Required(ErrorMessage = "Storage is required.")]
            public string Storage { get; set; }

            [Display(Name = "RAM")]
            [Required(ErrorMessage = "RAM is required.")]
            public string RAM { get; set; }

            [Display(Name = "Display")]
            [Required(ErrorMessage = "Display is required.")]
            public string Display { get; set; }

            [Display(Name = "Rear Camera")]
            [Required(ErrorMessage = "Rear Camera is required.")]
            public string RearCamera { get; set; }

            [Display(Name = "Front Camera")]
            [Required(ErrorMessage = "Front Camera is required.")]
            public string FrontCamera { get; set; }

            [Display(Name = "Battery")]
            [Required(ErrorMessage = "Battery is required.")]
            public string Battery { get; set; }

            [Display(Name = "Processor")]
            [Required(ErrorMessage = "Processor is required.")]
            public string Processor { get; set; }

            [Display(Name = "Operating System")]
            [Required(ErrorMessage = "Operating System is required.")]
            public string OS { get; set; }

            [Display(Name = "Dimensions")]
            [Required(ErrorMessage = "Dimensions are required.")]
            public string Dimensions { get; set; }

            [Display(Name = "Weight")]
            [Required(ErrorMessage = "Weight is required.")]
            public string Weight { get; set; }

            [Display(Name = "Price")]
            [Required(ErrorMessage = "Price is required.")]
            [DataType(DataType.Currency)]
            public decimal Price { get; set; }

            public IFormFile Avatar { get; set; }

            public byte[]? Image { get; set; }
    }
    }

