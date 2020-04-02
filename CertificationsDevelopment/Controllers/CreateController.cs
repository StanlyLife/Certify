using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CertificationsDevelopment.Interfaces;
using CertificationsDevelopment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CertificationsDevelopment.Controllers
{
    public class CreateController : Controller {
        public IEnumerable<SelectListItem> Subjects { get; set; }

        public readonly ICertificationData certData;
        public readonly IHtmlHelper htmlHelper;
        private readonly IFileUploadData fileData;

        private CertificationsModel Certification { get; set; }

        public CreateController(ICertificationData certData, IHtmlHelper htmlHelper, IFileUploadData fileData) {
            this.certData = certData;
            this.htmlHelper = htmlHelper;
            this.fileData = fileData;
        }

        [HttpGet]
        [Authorize]
        [Route("Certification/Update/{id:int}")]
        public IActionResult UpdateCertifications(int Id) {
            Certification = certData.GetById(Id);
            if (Certification.Author == User.Identity.Name) {

                ViewData["Function"] = $"Update certification: '{Certification.CertName}'";
                return View(Certification);
            } else {
                ViewData["Message"] = $"You are not allowed to edit this certification as you are not the owner";
                return RedirectToAction("index", "home");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Route("Certification/Update/{id:int}")]
        /*WILL id WORK AS WE DO NOT USE ROUTE HERE?*/
        public IActionResult UpdateCertifications(IFormFile file, CertificationsModel updatedCertification) {
            Certification = updatedCertification;

            if (!ModelState.IsValid) {
                return View(Certification);
            }

            if (file == null && Certification.CertUrl != null) {
                //update with URL
                if (IsUsingFile(Certification)) {
                    fileData.Delete(Certification.Id);
                }
                UpdateCertification(Certification, Certification.Id);
                return RedirectToAction("index", "home");
            } else if (file != null && Certification.CertUrl == null) {

                if (!IsUsingFileFormatAndSize(file)) {
                    ModelState.AddModelError("All", "File must be .pdf, .png, jpg or jpeg");
                    ModelState.AddModelError("All", "File must be under 500kb");
                    /*Return with data intact?*/
                    return View(Certification);
                }

                if (IsUsingCertUrl(Certification)) {
                    Certification.CertUrl = null;
                }
                if (IsUsingFile(Certification)) {
                    fileData.Delete(Certification.Id);
                }
                if (CreateFile(file, Certification)) {
                    UpdateCertification(Certification, Certification.Id);
                    return RedirectToAction("index", "home");
                } else {
                    TempData["Message"] = "The file uploaded already exists!";
                    return RedirectToAction("index", "home");
                }
            } else {
                if (file != null && Certification.CertUrl != null) {
                    ModelState.AddModelError("All", "Cannot use both file upload and certification url to display certificate, please choose one");
                    return View(Certification);
                } else {
                    if (IsUsingFile(Certification)) {
                        UpdateCertification(Certification, Certification.Id);
                        return RedirectToAction("index", "home");
                    }
                    ModelState.AddModelError("All", "You must upload a certification or add a link to the certification image");
                    return View(Certification);
                }
            }
            TempData["Error"] = "Something wrong happened, please try again";
            return RedirectToAction("index","error");
        }

        [HttpGet]
        [Route("Certification/Edit")]
        public IActionResult ListEditCertifications() {
            ViewData["Function"] = $"{User.Identity.Name}";
            var CertList = certData.GetCertificationsByAuthor(User.Identity.Name);
            if (CertList.Count() >= 1) {
                return View(CertList);
            }
            TempData["Message"] = "You do not have any certifications to edit";
            return RedirectToAction("index", "Home");
        }
        [HttpGet]
        [Authorize]
        [Route("Certification/Create/{CertificationId:int?}")]
        public IActionResult CreateCertifications() {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Route("Certification/Create/{CertificationId:int?}")]
        public IActionResult CreateCertifications(CertificationsModel newCertification, IFormFile file, int? CertificationId) {

            //CHECK MODEL STATE

            Certification = newCertification;
            if (!ModelState.IsValid) {
                return View(Certification);
            }
            if(file == null && Certification.CertUrl != null) {
                CreateCertification(Certification);
            }else if(file != null && Certification.CertUrl == null) {
                if (!IsUsingFileFormatAndSize(file)) {
                    ModelState.AddModelError("All", "File must be .pdf, .png, jpg or jpeg");
                    ModelState.AddModelError("All", "File must be under 500kb");
                    return View(Certification);
                }
                var createdCertification = CreateCertification(Certification);
                if (CreateFile(file, createdCertification)) {
                    return RedirectToAction("index","home");
                } else {
                    DeleteCertificationNoAuth(createdCertification);
                    TempData["Message"] = "The file uploaded already exists!";
                    return RedirectToAction("index", "home");
                }

            } else if (file != null && Certification.CertUrl != null) {
                ModelState.AddModelError("All","Cannot use both file upload and certification url to display certificate, please choose one");
                return View(Certification);
            } else {
                ModelState.AddModelError("All" ,"You must upload a certification or add a link to the certification image");
                return View(Certification);
            }

            TempData["Message"] = $"Created certification '{Certification.CertName}'";
            return RedirectToAction("index", "home");
        }


        [HttpGet]
        [Authorize]
        public void DeleteCertificationNoAuth(CertificationsModel cert) {
            certData.Delete(cert.Id);
            certData.Commit();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [Route("Certification/Delete/{id:int}")]
        public IActionResult DeleteCertificationPost(int id) {
            var CertToDelete = certData.GetById(id);
            if (CertToDelete.Author == User.Identity.Name) {
            DeleteCertificationNoAuth(CertToDelete);
            TempData["Message"] = "Certification deleted";
            return RedirectToAction("ListEditCertifications", "Create");
            }
            TempData["Message"] = "You do not have permission to delete this certification";
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        [Authorize]
        [Route("Certification/Delete/{id:int}")]
        public IActionResult DeleteCertification(int? id) {
            Certification = certData.GetById(id.Value);
            if(id.HasValue && Certification != null) {
                return View(Certification);
            } else {
                TempData["Message"] = "This page does not exist!";
                return RedirectToAction("index","home");
            }
        }

        public CertificationsModel UpdateCertification(CertificationsModel newCertification, int CertificationId) {
            var Certification = certData.GetById(CertificationId);
            Certification.CertName = newCertification.CertName;
            Certification.CertSubject = newCertification.CertSubject;
            Certification.CertSite = newCertification.CertSite;
            Certification.CertDescription = newCertification.CertDescription;
            Certification.CertUrl = newCertification.CertUrl;


            certData.Update(Certification);
            certData.Commit();
            TempData["Message"] = $"Certification '{Certification.CertName}' Updated!";
            return Certification;
        }

        public CertificationsModel CreateCertification(CertificationsModel newCertification) {

            newCertification.Author = User.Identity.Name;
            newCertification.Posted = DateTime.Now;

            var CreatedCertification = certData.Add(newCertification);
            certData.Commit();

            return CreatedCertification;
        }

        public bool CreateFile(IFormFile file, CertificationsModel cert) {
            Fileupload img = new Fileupload();
            img.FileName = file.FileName;
            img.Author = User.Identity.Name;
            img.DateAdded = DateTime.Now;
            img.CertificationId = cert.Id;

            MemoryStream ms = new MemoryStream();
            file.CopyTo(ms);
            img.FileData = ms.ToArray();

            if (fileData.DoesFileExist(img)) {
                return false;
            }


            ms.Close();
            ms.Dispose();

            fileData.Add(img);
            fileData.Commit();

            return true;

        }

        public bool IsUsingCertUrl(CertificationsModel cert) {
            if (cert.CertUrl == null) {
                return false;
            }
            return true;
        }

        public bool IsUsingFile(CertificationsModel cert) {
            if(fileData.GetByCertId(cert.Id) == null) {
                return false;
            }
            return true;
        }

        public bool IsUsingFileFormatAndSize(IFormFile file) {
            if (file.Length < 510000 && (file.FileName.EndsWith(".pdf") || file.FileName.EndsWith(".png") || file.FileName.EndsWith(".jpg") || file.FileName.EndsWith(".jpeg"))) {
                return true;
            }
            return false;
        }

    }
}