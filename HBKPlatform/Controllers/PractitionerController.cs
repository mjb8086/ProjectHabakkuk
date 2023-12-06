/******************************
* HBK Practitioner Controller
* Author: Mark Brown
* Authored: 10/09/2022
******************************/

using HBKPlatform.Helpers;
using HBKPlatform.Models;
using HBKPlatform.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Controllers
{
    public class PractitionerController : Controller
    {
        private readonly IPractitionerRepository _practitionerRepository;

        public PractitionerController(IPractitionerRepository practitionerRepository)
        {
            _practitionerRepository = practitionerRepository;
        }

        [HttpGet]
        [Route("practitioner/{idx:int}")]
        public IActionResult Index(int idx)
        {
            var practitioner = _practitionerRepository.GetPractitioner(idx);
            if (idx >=0 && practitioner != null)
            {
//                return View(_practitionerRepository.GetPractitionerView(practitioner));
                return Ok("Ok");
            }
            else
            {
                return NotFound();
            }
        }

    }
}

