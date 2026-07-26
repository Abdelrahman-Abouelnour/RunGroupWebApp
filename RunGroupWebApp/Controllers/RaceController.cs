using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.Repository;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IPhotoService _photoService;

        public RaceController(IRaceRepository raceRepository, IPhotoService photoService)
        {
            _raceRepository = raceRepository;
            _photoService = photoService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Race> race = await _raceRepository.GetAll();
            return View(race);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Race race = await _raceRepository.GetByIdAsync(id);
            return View(race);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel raceVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(raceVM.Image);
                var race = new Race
                {
                    Title = raceVM.Title,
                    Description = raceVM.Description,
                    Image = result.Url.ToString(),
                    RaceCategory = raceVM.RaceCategory,
                    Address = new Address { City = raceVM.Address.City, State = raceVM.Address.State, Street = raceVM.Address.Street }
                };
                _raceRepository.Add(race);
                return RedirectToAction("Index");

            }
            else
            {
                ModelState.AddModelError("", "Failure during photo upload");
            }
            return View(raceVM);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var race = await _raceRepository.GetByIdAsync(id);
            if (race == null) return View("Error");
            var raceVM = new EditRaceViewModel
            {
                Id = race.Id,
                Title = race.Title,
                Description = race.Description,
                AddressId = race.AddressId,
                address = race.Address,
                Url = race.Image,
                raceCategory = race.RaceCategory,


            };
            return View(raceVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRaceViewModel raceVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit Model)");
                return View("Edit", raceVM);
            }
            var userRace = await _raceRepository.GetByIdAsyncNoTracking(id);
            if (userRace != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(userRace.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                    return View(raceVM);
                }
                var photoResult = await _photoService.AddPhotoAsync(raceVM.Image);

                var race = new Race
                {
                    Id = id,
                    Title = raceVM.Title,
                    Description = raceVM.Description,
                    Image = photoResult.Url.ToString(),
                    RaceCategory = raceVM.raceCategory,
                    AddressId = raceVM.AddressId,
                    Address = raceVM.address
                };
                _raceRepository.Update(race);

                return RedirectToAction("Index");
            }
            else
            {
                return View(raceVM);
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            var raceToDelete = await _raceRepository.GetByIdAsync(id);
            if (raceToDelete == null) return View("Error");
            return View(raceToDelete);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteRace(int id)
        {
            var raceToDelete = await _raceRepository.GetByIdAsync(id);
            if (raceToDelete == null) return View("Error");
            _raceRepository.Delete(raceToDelete);
            return RedirectToAction("Index");
        }
    }
}
