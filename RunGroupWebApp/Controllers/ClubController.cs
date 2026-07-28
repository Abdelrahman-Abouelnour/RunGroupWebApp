using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;
using System.Diagnostics.Eventing.Reader;

namespace RunGroupWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContext;
        public ClubController(IClubRepository clubRepository, IPhotoService photoService, IHttpContextAccessor httpContext)
        {
            _clubRepository = clubRepository;
            _photoService = photoService;
            _httpContext = httpContext;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Club> clubs = await _clubRepository.GetAll();
            return View(clubs);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Club club = await _clubRepository.GetByIdAsync(id);
            return View(club);
        }
        public IActionResult Create()
        {
            var curUserId = _httpContext.HttpContext.User.GetUserId();
            var curUserVM = new CreateClubViewModel
            {
                AppUserId = curUserId.ToString()
            };
            return View(curUserVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel clubVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(clubVM.Image);
                var club = new Club
                {
                    Title = clubVM.Title,
                    Description = clubVM.Description,
                    Image = result.Url.ToString(),
                    AppUserId = clubVM.AppUserId,
                    ClubCategory = clubVM.ClubCategory,
                    Address = new Address { City = clubVM.Address.City, State = clubVM.Address.State, Street = clubVM.Address.Street }
                };
                _clubRepository.Add(club);
                return RedirectToAction("Index");

            }
            else
            {
                ModelState.AddModelError("", "Failure during photo upload");
            }
            return View(clubVM);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null) return View("Error");
            var clubVM = new EditClubViewModel
            {
                Title = club.Title,
                Description = club.Description,
                AddressId = club.AddressId,
                address = club.Address,
                Url = club.Image,
                clubCategory = club.ClubCategory,


            };
            return View(clubVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubViewModel clubVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit Model)");
                return View("Edit", clubVM);
            }
            var userClub = await _clubRepository.GetByIdAsyncNoTracking(id);
            if (userClub != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(userClub.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                    return View(clubVM);
                }
                var photoResult = await _photoService.AddPhotoAsync(clubVM.Image);

                var club = new Club
                {
                    Id = id,
                    Title = clubVM.Title,
                    Description = clubVM.Description,
                    Image = photoResult.Url.ToString(),
                    ClubCategory = clubVM.clubCategory,
                    AddressId = clubVM.AddressId,
                    Address = clubVM.address
                };
                _clubRepository.Update(club);

                return RedirectToAction("Index");
            }
            else
            {
                return View(clubVM);
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            var clubToDelete = await _clubRepository.GetByIdAsync(id);
            if (clubToDelete == null) return View("Error");
            return View(clubToDelete);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            var clubToDelete = await _clubRepository.GetByIdAsync(id);
            if (clubToDelete == null) return View("Error");
            _clubRepository.Delete(clubToDelete);
            return RedirectToAction("Index");
        }

    }
}
