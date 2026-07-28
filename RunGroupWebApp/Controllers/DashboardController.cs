using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IPhotoService _photoService;
        public DashboardController(IDashboardRepository dashboardRepository, IHttpContextAccessor httpContext, IPhotoService photoService)
        {
            _dashboardRepository = dashboardRepository;
            _httpContext = httpContext;
            _photoService = photoService;
        }
        private void Mapper2000(AppUser user,EditUserDashboardViewModel editVM, ImageUploadResult uploadResult)
        {
            user.Id = editVM.Id;
            user.Pace = editVM.Pace;
            user.Mileage = editVM.Mileage;
            user.ProfileImageUrl = uploadResult.Url.ToString();
            user.City = editVM.City;
            user.State = editVM.State;
        }
        public async Task<IActionResult> Index()
        {
            var userClubs = await _dashboardRepository.GetAllUserClubs();
            var userRaces = await _dashboardRepository.GetAllUserRaces();
            var dashboardViewModel = new DashboardVM
            {
                Clubs = userClubs,
                Races = userRaces
            };
            return View(dashboardViewModel);
        }
        public async Task<IActionResult> EditUserProfile()
        {
            var curUserId = _httpContext.HttpContext.User.GetUserId();
            var user = await _dashboardRepository.GetUserById(curUserId);
            if (user == null) return View("Error");
            var editUserVM = new EditUserDashboardViewModel()
            {
                Id = curUserId,
                Pace = user.Pace,
                Mileage = user.Mileage,
                ProfileImageUrl = user.ProfileImageUrl,
                City = user.City,
                State = user.State
            };
            return View(editUserVM);
        }
        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditUserDashboardViewModel editUserVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View("EditUserProfile", editUserVM);
            }

            var user = await _dashboardRepository.GetUserByIdNoTracking(editUserVM.Id);
            if (user.ProfileImageUrl == "" || user.ProfileImageUrl == null)
            {
                var photoResult = await _photoService.AddPhotoAsync(editUserVM.Image);
                Mapper2000(user, editUserVM, photoResult);
                _dashboardRepository.Update(user);
                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    await _photoService.DeletePhotoAsync(user.ProfileImageUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not Delete photo");
                    return View(editUserVM);
                }
                var photoResult = await _photoService.AddPhotoAsync(editUserVM.Image);
                Mapper2000(user, editUserVM, photoResult);
                _dashboardRepository.Update(user);
                return RedirectToAction("Index");

            }
        }
    }
}
