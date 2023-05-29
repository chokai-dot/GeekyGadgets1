using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekyGadgets.DAL.Interfaces;
using GeekyGadgets.Domain.Entity;
using GeekyGadgets.Domain.Enum;
//using GeekyGadgets.Domain.Extensions;
using GeekyGadgets.Domain.Response;

using GeekyGadgets.Domain.ViewModels.Smartphone;
using GeekyGadgets.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeekyGadgets.Service.Implementations
{
    public class SmartphoneService : ISmartphoneService
    {
        private readonly IBaseRepository<Smartphone> _smartphoneRepository;

        public SmartphoneService(IBaseRepository<Smartphone> smartphoneRepository)
        {
            _smartphoneRepository = smartphoneRepository;
        }

      

        public async Task<IBaseResponse<SmartphoneViewModel>> GetSmartphone(int id)
        {
            try
            {
                var smartphone = await _smartphoneRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                if (smartphone == null)
                {
                    return new BaseResponse<SmartphoneViewModel>()
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.UserNotFound
                    };
                }

                var data = new SmartphoneViewModel()
                {
                    Id = smartphone.Id,
                    Brand = smartphone.Brand,
                    Model = smartphone.Model,
                    Color = smartphone.Color,
                    Storage = smartphone.Storage,
                    RAM = smartphone.RAM,
                    Display = smartphone.Display,
                    RearCamera = smartphone.RearCamera,
                    FrontCamera = smartphone.FrontCamera,
                    Battery = smartphone.Battery,
                    Processor = smartphone.Processor,
                    OS = smartphone.OS,
                    Dimensions = smartphone.Dimensions,
                    Weight = smartphone.Weight,
                    Price = smartphone.Price,
                    Image=smartphone.Avatar,
                };

                return new BaseResponse<SmartphoneViewModel>()
                {
                    StatusCode = StatusCode.OK,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<SmartphoneViewModel>()
                {
                    Description = $"[GetCar] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<Dictionary<int, string>>> GetSmartphone(string term)
        {
            var baseResponse = new BaseResponse<Dictionary<int, string>>();
            try
            {
                var smartphones = await _smartphoneRepository.GetAll()
                .Select(x => new SmartphoneViewModel()
                {
                    Id = x.Id,
                    Brand = x.Brand,
                    Model = x.Model,
                    Color = x.Color,
                    Storage = x.Storage,
                    RAM = x.RAM,
                    Display = x.Display,
                    RearCamera = x.RearCamera,
                    FrontCamera = x.FrontCamera,
                    Battery = x.Battery,
                    Processor = x.Processor,
                    OS = x.OS,
                    Dimensions = x.Dimensions,
                    Weight = x.Weight,
                    Price = x.Price,
                    
                    
                })
                .Where(x => EF.Functions.Like(x.Brand, $"%{term}%") || EF.Functions.Like(x.Model, $"%{term}%"))
                .ToDictionaryAsync(x => x.Id, t => t.Brand + " " + t.Model);
                baseResponse.Data = smartphones;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<Dictionary<int, string>>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<Smartphone>> Create(SmartphoneViewModel model, byte[] imageData)
        {
            try
            {
                var smartphone = new Smartphone()
                {
                    Brand = model.Brand,
                    Model = model.Model,
                    Color = model.Color,
                    Storage = model.Storage,
                    RAM = model.RAM,
                    Display = model.Display,
                    RearCamera = model.RearCamera,
                    FrontCamera = model.FrontCamera,
                    Battery = model.Battery,
                    Processor = model.Processor,
                    OS = model.OS,
                    Dimensions = model.Dimensions,
                    Weight = model.Weight,
                    Price = model.Price,
                    Avatar = imageData
                };

                await _smartphoneRepository.Create(smartphone);

                return new BaseResponse<Smartphone>()
                {
                    StatusCode = StatusCode.OK,
                    Data = smartphone
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Smartphone>()
                {
                    Description = $"[Create] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }



        public async Task<IBaseResponse<bool>> DeleteSmartphone(int id)
        {
            try
            {
                var smartphone = await _smartphoneRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                if (smartphone == null)
                {
                    return new BaseResponse<bool>()
                    {
                        Description = "User not found",
                        StatusCode = StatusCode.UserNotFound,
                        Data = false
                    };
                }

                /*await _carRepository.Delete(car);*/

                return new BaseResponse<bool>()
                {
                    Data = true,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>()
                {
                    Description = $"[DeleteSmartphone] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<Smartphone>> Edit(int id, SmartphoneViewModel model)
        {
            try
            {
                var smartphone = await _smartphoneRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                if (smartphone == null)
                {
                    return new BaseResponse<Smartphone>()
                    {
                        Description = "Smartphone not found",
                        StatusCode = StatusCode.ProductNotFound
                    };
                }

                smartphone.Brand = model.Brand;
                smartphone.Model = model.Model;
                smartphone.Color = model.Color;
                smartphone.Storage = model.Storage;
                smartphone.RAM = model.RAM;
                smartphone.Display = model.Display;
                smartphone.RearCamera = model.RearCamera;
                smartphone.FrontCamera = model.FrontCamera;
                smartphone.Battery = model.Battery;
                smartphone.Processor = model.Processor;
                smartphone.OS = model.OS;
                smartphone.Dimensions = model.Dimensions;
                smartphone.Weight = model.Weight;
                smartphone.Price = model.Price;
                
                await _smartphoneRepository.Update(smartphone);

                return new BaseResponse<Smartphone>()
                {
                    Data = smartphone,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Smartphone>()
                {
                    Description = $"[Edit] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }


        public IBaseResponse<List<Smartphone>> GetSmartphones()
        {
            try
            {
                var smartphones = _smartphoneRepository.GetAll().ToList();
                if (!smartphones.Any())
                {
                    return new BaseResponse<List<Smartphone>>()
                    {
                        Description = "Найдено 0 элементов",
                        StatusCode = StatusCode.OK
                    };
                }

                return new BaseResponse<List<Smartphone>>()
                {
                    Data = smartphones,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<Smartphone>>()
                {
                    Description = $"[GetSmartphones] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

      
    }
}