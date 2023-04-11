using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GeekyGadgets.Domain.ViewModels.Laptop
{
    public class LaptopViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле 'Бренд' обязательно для заполнения.")]
        [Display(Name = "Бренд")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Поле 'Модель' обязательно для заполнения.")]
        [Display(Name = "Модель")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Поле 'Модель процессора' обязательно для заполнения.")]
        [Display(Name = "Модель процессора")]
        public string ProcessorModel { get; set; }

        [Required(ErrorMessage = "Поле 'Количество ядер процессора' обязательно для заполнения.")]
        [Display(Name = "Количество ядер процессора")]
        public int ProcessorCores { get; set; }

        [Required(ErrorMessage = "Поле 'Скорость процессора' обязательно для заполнения.")]
        [Display(Name = "Скорость процессора")]
        public float ProcessorSpeed { get; set; }

        [Display(Name = "Видеокарта")]
        public string GraphicsCard { get; set; }

        [Display(Name = "Тип видеокарты")]
        public string GraphicsCardType { get; set; }

        [Required(ErrorMessage = "Поле 'Размер экрана' обязательно для заполнения.")]
        [Display(Name = "Размер экрана")]
        public float ScreenSize { get; set; }

        [Required(ErrorMessage = "Поле 'Разрешение экрана' обязательно для заполнения.")]
        [Display(Name = "Разрешение экрана")]
        public string Resolution { get; set; }

        [Required(ErrorMessage = "Поле 'Объем хранилища' обязательно для заполнения.")]
        [Display(Name = "Объем хранилища")]
        public string Storage { get; set; }

        [Required(ErrorMessage = "Поле 'Объем оперативной памяти' обязательно для заполнения.")]
        [Display(Name = "Объем оперативной памяти")]
        public string Memory { get; set; }

        [Required(ErrorMessage = "Поле 'Цена' обязательно для заполнения.")]
        [Display(Name = "Цена")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Поле 'Год выпуска' обязательно для заполнения.")]
        [Display(Name = "Год выпуска")]
        public int Year { get; set; }
    }

}
