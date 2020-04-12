using System;
using System.IO;
using CertificationsDevelopment.Interfaces;
using CertificationsDevelopment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CertificationsDevelopment.Controllers
{
	public class ProfileController : Controller {
		private readonly IUserProfileData profileData;
		private readonly ICertificationData certData;
		private readonly UserManager<IdentityUser> userManager;
		private readonly IHtmlHelper html;

		public ProfileController(IUserProfileData profileData, ICertificationData certData, UserManager<IdentityUser> UserManager, IHtmlHelper html) {
			this.profileData = profileData;
			this.certData = certData;
			userManager = UserManager;
			this.html = html;
		}

		
		[HttpGet]
		[Authorize]
		[Route("Profile/Manage/{userKey}")]
		public IActionResult Manage(string userKey)
		{
			var user = userManager.FindByNameAsync(User.Identity.Name).Result;
			string thisUserId = user.Id.ToString().ToLower();
			string foreignUserId = userKey.ToString().ToLower();
			if (thisUserId == foreignUserId) {
				UserProfile profileModel = new UserProfile();
				profileModel = profileData.GetProfileByEmail(User.Identity.Name);
				profileModel.user = user;
				return View(profileModel);
			}

			TempData["Message"] = $"You can NOT view this, thisUserId is {thisUserId}, foreignUserId is {foreignUserId}";
			return RedirectToAction("index","home");
		}
		
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		[Route("Profile/Manage/{userKey}")]
		public IActionResult Manage(UserProfile profile, [FromForm(Name = "file")] IFormFile image, [FromForm(Name = "privacy")]bool privacy)
		{
			var user = userManager.FindByNameAsync(User.Identity.Name).Result;
			bool profileAlreadyExists = true;

			if (profile.Website != null) {
				profile.Website = FixUrl(profile.Website);
			}
			UserProfile profileModel = profileData.GetProfileByUserId(user.Id);
			profileModel.user = user;


			if (ModelState.IsValid) {

				if (image == null && profileModel.ProfileImage == null) { 
					TempData["Error"] = "You need to upload a profile picture";
					ModelState.AddModelError("All", "You need to upload a profile picture");
					return View(profileModel);
				}
				if (image != null) {
					if (!ValidateImage(image)) {
						ModelState.AddModelError("All","Image must be valid format and size");
						return View(profileModel);
					}
					AddImageToModel(image, profileModel);
				}

				profileModel.UserKey = user.Id;
				TryUpdateModelAsync(profileModel);
				profileModel.IsPrivate = privacy;

				if (profileAlreadyExists) {
					UpdateProfile(profileModel);
				} else {
					AddProfile(profileModel);
				}
				return RedirectToAction("ProfileDetails",new {id = profileModel.ProfileId });
			}else {
				TempData["Error"] = "Profile was not updated";
				return View(profileModel);
			}
		}

		[HttpGet]
		[Route("Profile/user/{id:int}")]
		public IActionResult ProfileDetails(int id) {
			UserProfile userProfile = new UserProfile();
			userProfile = profileData.GetProfileByID(id);
			if (userProfile.UserKey == null) {
				TempData["Message"] = "This profile does not exist";
				return RedirectToAction("index","home");
			}else if (userProfile.IsPrivate && userProfile.UserKey != userManager.FindByNameAsync(User.Identity.Name).Result.Id) {
				TempData["Message"] = "This profile is private";
				return RedirectToAction("index","home");
			}

			if (userProfile.ProfileImage == null) {
				TempData["Message"] = "you need to update your profile to view it!";
				return RedirectToAction("index", "home");
			}

			if (userProfile.IsPrivate) {
				TempData["Message"] = "Your profile is private";
			}

			userProfile.user = userManager.FindByIdAsync(userProfile.UserKey).Result;
			userProfile.ProfileImageUrl = GetImageUrl(userProfile);
			userProfile.Certifications = certData.GetCertificationsByAuthor(userProfile.user.Email);
			return View(userProfile);
		}


		public string GetImageUrl(UserProfile _profileImage) {
			string imageDataBytes = Convert.ToBase64String(_profileImage.ProfileImage);
			string imageUrl = string.Format("data:/image/jpeg;base64,{0}", imageDataBytes);
			return imageUrl;
		}
		public void AddProfile(UserProfile profileModel) {
			profileModel.email = User.Identity.Name;
			profileData.Add(profileModel);
			profileData.Commit();
			TempData["Message"] = "Profile created sucsessfully!";
		}

		public void UpdateProfile(UserProfile profileModel) {
			profileData.Update(profileModel);
			profileData.Commit();
			TempData["Message"] = "Profile updated sucsessfully!";
		}
		public void AddImageToModel(IFormFile file, UserProfile profile) {
			MemoryStream ms = new MemoryStream();
			file.CopyTo(ms);
			profile.ProfileImage = ms.ToArray();
			ms.Close();
			ms.Dispose();
		}

		public bool ValidateImage(IFormFile image) {
			if (image.Length > 510000) {
				TempData["Error"] = "File must be under 500kb and be jpeg, jpg or png format";
				return false;
			}else if (image.FileName.EndsWith("png") || image.FileName.EndsWith("jpg") || image.FileName.EndsWith("jpeg")) {
				return true;
			}
				TempData["Error"] = "File must be jpeg, jpg or png format";
				return false;
		}

		public string FixUrl(string url) {
			if (url.Contains("www.") || url.StartsWith("https://")) {
				return url;
			}
			url.Insert(0, "www.");
			return url;
		}
	}
}