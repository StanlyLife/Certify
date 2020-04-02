using CertificationsDevelopment.Interfaces;
using CertificationsDevelopment.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CertificationsDevelopment.Controllers {
	public class CertificationsController : Controller {
		public ICertificationData CertData { get; }

		private readonly IUserProfileData profileData;
		private IFileUploadData fileData;
		public CertificationsModel Certification { get; set; }
		public Fileupload image;
		public CertificationsController(ICertificationData certData,IUserProfileData profileData, IFileUploadData fileData) {
			CertData = certData;
			this.profileData = profileData;
			this.fileData = fileData;
		}

		// GET: Certifications
		[Route("Certifications/{search?}")]
		public ActionResult Index(string? search) {
			SearchModel model = SearchCertificationsAndProfiles(search);
			return View(model);
		}

		[Route("Certification/{id:int}")]
		public IActionResult Details(int id) {
			Certification = new CertificationsModel();
			Certification = CertData.GetById(id);
			image = new Fileupload();
			image = fileData.GetByCertId(id).FirstOrDefault();
			ViewBag.story = JsonConvert.SerializeObject(new String(""));

			if (Certification.CertUrl != null) {
				return View(Certification);
			}

			if (Certification == null) {
				TempData["Error"] = $"Certification does not exist";
				return RedirectToAction("Index","Certifications");
			}

			if(Certification.CertUrl == null && image != null) {
				System.Console.WriteLine("No Certification url");

				string imageDataBytes = Convert.ToBase64String(image.FileData);

				//IF UPLOADED IS PNG
				if (image.FileName.Contains(".pdf")) {
					ViewBag.story = JsonConvert.SerializeObject(imageDataBytes);

				}else {
					string imageUrl = string.Format("data:/image/jpeg;base64,{0}", imageDataBytes); //Original
					Certification.CertUrl = imageUrl;
				}
			}else {
				TempData["Error"] = $"Certification '{Certification.CertName}' with Id: {Certification.Id} does not have a certification";
				return RedirectToAction("Index", "Certifications");
			}

			return View(Certification);
		}

		public SearchModel SearchCertificationsAndProfiles(string search) {
			SearchModel model = new SearchModel();

			
			model.search = search;

			if (!string.IsNullOrWhiteSpace(search)) {
				var profile = profileData.GetProfileByEmail(search);
				if (!profile.IsPrivate) {
					model.profile = profile;

					model.profile.ProfileImageUrl = GetImageUrl(model.profile);

					model.Certifications = CertData.GetCertificationsByAuthor(search);
				} else if(profile.ProfileId != 0) {
					model.profile = new UserProfile();
					model.Certifications = CertData.GetCertificationsByName(search);
				}else {
					model.profile = null;
					model.Certifications = CertData.GetCertificationsByName(search);
				}
			}else {
				//If search is null
				model.Certifications = CertData.GetAll();
			}

			return model;

		}

		public string GetImageUrl(UserProfile _profileImage) {
			string imageDataBytes = Convert.ToBase64String(_profileImage.ProfileImage);
			string imageUrl = string.Format("data:/image/jpeg;base64,{0}", imageDataBytes);
			return imageUrl;
		}

	}

}