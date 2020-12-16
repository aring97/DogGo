using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogGo.Repositories;
using DogGo.Models;
using DogGo.Models.ViewModels;

namespace DogGo.Controllers
{
    public class WalkerController : Controller
    {
        private readonly IWalkerRepository _walkerRepo;
        private readonly IWalksRepository _walksRepo;
        private readonly IDogRepository _dogRepo;
        private readonly IOwnerRepository _ownerRepo;

        // ASP.NET will give us an instance of our Walker Repository. This is called "Dependency Injection"
        public WalkerController(IWalkerRepository walkerRepository, IWalksRepository walksRepository, IDogRepository dogRepository, IOwnerRepository ownerRepository)
        {
            _walkerRepo = walkerRepository;
            _walksRepo = walksRepository;
            _dogRepo = dogRepository;
            _ownerRepo = ownerRepository;
        }
        // GET: WalkerController
        public ActionResult Index()
        {
            List<Walker> walkers = _walkerRepo.GetAllWalkers(); 
            return View(walkers);
        }

        // GET: WalkerController/Details/5
        public ActionResult Details(int id)
        {
            Walker walker = _walkerRepo.GetWalkerById(id);
            if (walker == null)
            {
                return NotFound();
            }
            WalkerProfileViewModel vm = createViewModel(id, walker);
            return View(vm);
        }

        // GET: WalkerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WalkerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkerController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: WalkerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: WalkerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private WalkerProfileViewModel createViewModel(int walkerId, Walker walker)
        {
            List<Walks> unsortedWalks = _walksRepo.GetWalksByWalkerId(id);
            List<Dog> unsortedDogs = _dogRepo.GetAllDogs();
            List<Owner> sortedOwners = _ownerRepo.GetOwnersSortedByName();
            List<Dog> sortedDogs = new List<Dog>();
            List<Walks> sortedWalks = new List<Walks>();
            List<Owner> relaventOwners = new List<Owner>();
            List<Dog> relaventDogs = new List<Dog>();
            foreach (Owner owner in sortedOwners)
            {
                foreach (Dog dog in unsortedDogs)
                {
                    if (dog.OwnerId == owner.id)
                    {
                        sortedDogs.Add(dog);
                    }
                }
            }
            foreach (Dog dog in sortedDogs)
            {
                foreach (Walks walk in unsortedWalks)
                {
                    if (dog.Id == walk.DogId)
                    {
                        sortedWalks.Add(walk);
                    }
                }
            }
            foreach (Walks walk in sortedWalks)
            {
                foreach (Dog dog in sortedDogs)
                {
                    if (walk.DogId == dog.Id)
                    {
                        relaventDogs.Add(dog);
                    }
                }
            }
            foreach (Dog dog in relaventDogs)
            {
                foreach (Owner owner in sortedOwners)
                {
                    if (dog.OwnerId == owner.id)
                    {
                        relaventOwners.Add(owner);
                    }
                }
            }
            WalkerProfileViewModel vm = new WalkerProfileViewModel
            {
                Walker = walker,
                Walk = sortedWalks,
                Owner = relaventOwners
            };
            return vm;
        }
    }
}
