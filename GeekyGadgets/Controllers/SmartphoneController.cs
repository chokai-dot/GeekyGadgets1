using GeekyGadgets.Domain.ViewModels.Smartphone;
using GeekyGadgets.Service.Implementations;
using GeekyGadgets.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace GeekyGadgets.Controllers
{
    public class SmartphoneController : Controller
    {
        private readonly ISmartphoneService _smartphoneService;

        public SmartphoneController(ISmartphoneService smartphoneService)
        {
            _smartphoneService = smartphoneService;
        }

        [HttpGet]
        public IActionResult GetSmartphones()
        {
            var response = _smartphoneService.GetSmartphones();
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);
            }
            ViewBag.ErrorMessage = response.Description;
            return View(response.Data);


        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _smartphoneService.DeleteSmartphone(id);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return RedirectToAction("GetSmartphones");
            }
            return View("Error", $"{response.Description}");
        }

        public IActionResult Compare() => PartialView();

        [HttpGet]
        public async Task<IActionResult> Save(int id)
        {
            if (id == 0)
                return PartialView();

            var response = await _smartphoneService.GetSmartphone(id);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return PartialView(response.Data);
            }
            ModelState.AddModelError("", response.Description);
            return PartialView();
        }

        // string Name, string Model, double Speed, string Description, decimal Price, string TypeCar, IFormFile Avatar
        [HttpPost]
        public async Task<IActionResult> Save(SmartphoneViewModel viewModel)
        {
            ModelState.Remove("Id");
            ModelState.Remove("DateCreate");
            if (ModelState.IsValid)
            {
                if (viewModel.Id == 0)
                {
                    byte[] imageData;
                    using (var binaryReader = new BinaryReader(viewModel.Avatar.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)viewModel.Avatar.Length);
                    }
                    await _smartphoneService.Create(viewModel, imageData);
                }
                else
                {
                    await _smartphoneService.Edit(viewModel.Id, viewModel);
                }

                
                ModelState.Clear();
                ModelState.AddModelError("", "The smartphone has been saved successfully.");

                return RedirectToAction("GetSmartphones");
            }

            return View();
        }



        [HttpGet]
        public async Task<ActionResult> GetSmartphone(int id, bool isJson)
        {
            var response = await _smartphoneService.GetSmartphone(id);
            if (isJson)
            {
                return Json(response.Data);
            }
            return PartialView("GetSmartphone", response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> GetSmartphone(string term, int page = 1, int pageSize = 5)
        {
            var response = await _smartphoneService.GetSmartphone(term);
            return Json(response.Data);
        }
        
        //[HttpPost]
        //public JsonResult GetTypes()
        //{
        //    var types = _smartphoneService.GetTypes();
        //    return Json(types.Data);
        //}
    }
}
