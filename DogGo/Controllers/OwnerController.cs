﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogGo.Repositories;
using DogGo.Models;
using DogGo.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace DogGo.Controllers
{
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IDogRepository _dogRepo;
        private readonly IWalkerRepository _walkerRepo;
        private readonly INeighborhoodRepository _neighborhoodrepo;
        public OwnerController(IConfiguration config)
        {
            _ownerRepository = new OwnerRepository(config);
            _dogRepo = new DogRepository(config);
            _walkerRepo = new WalkerRepository(config);
            _neighborhoodrepo = new NeighborhoodRepository(config);
        }
        // GET: OwnerController
        public ActionResult Index()
        {
            List<Owner> owners = _ownerRepository.GetAllOwners();
            return View(owners);
        }

        // GET: OwnerController/Details/5
        public ActionResult Details(int id)
        {
            Owner owner = _ownerRepository.GetOwnerById(id);
            List<Dog> dogs = _dogRepo.GetDogsByOwnerId(owner.id);
            List<Walker> walkers = _walkerRepo.GetWalkersInNeighborhood(owner.NeighborhoodId);
            if (owner == null)
            {
                return NotFound();
            }
            ProfileViewModel vm = new ProfileViewModel()
            {
                Owner = owner,
                Dogs = dogs,
                Walkers = walkers
            };
            return View(vm);
        }

        // GET: OwnerController/Create
        public ActionResult Create()
        {
            List<Neighborhood> neighborhoods = _neighborhoodrepo.GetAll();
            OwnerFormViewModel vm = new OwnerFormViewModel()
            {
                Owner = new Owner(),
                Neighborhoods = neighborhoods
            };
            return View(vm);
        }

        // POST: OwnerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OwnerFormViewModel vm)
        {
            try
            {
                _ownerRepository.AddOwner(vm.Owner);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return View(vm);
            }
        }

        // GET: OwnerController/Edit/5
        public ActionResult Edit(int id)
        {
            Owner owner = _ownerRepository.GetOwnerById(id);
            List<Neighborhood> neighborhoods = _neighborhoodrepo.GetAll();
            if (owner == null)
            {
                return NotFound();
            }
            OwnerFormViewModel vm = new OwnerFormViewModel
            {
                Owner = owner,
                Neighborhoods = neighborhoods
            };
            return View(vm);
        }

        // POST: OwnerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OwnerFormViewModel vm)
        {
            try
            {
                _ownerRepository.UpdateOwner(vm.Owner);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return View(vm);
            }
        }

        // GET: OwnerController/Delete/5
        public ActionResult Delete(int id)
        {
            Owner owner = _ownerRepository.GetOwnerById(id);
            return View(owner);
        }

        // POST: OwnerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Owner owner)
        {
            try
            {
                _ownerRepository.DeleteOwner(id);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return View(owner);
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel viewModel)
        {
            Owner owner = _ownerRepository.GetOwnerByEmail(viewModel.Email);

            if (owner == null)
            {
                return Unauthorized();
            }

            List<Claim> claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, owner.id.ToString()),
        new Claim(ClaimTypes.Email, owner.Email),
        new Claim(ClaimTypes.Role, "DogOwner"),
    };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Dog");
        }
    }
}
