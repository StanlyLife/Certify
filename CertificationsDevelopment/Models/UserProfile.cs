using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CertificationsDevelopment.Models {
	public class UserProfile {
		
		public UserProfile() {
			IsPrivate = true;
		}

		[Key]
		public int ProfileId { get; set; }
		public string UserKey{get; set;}

		[Required(ErrorMessage = "First name is required")]
		public string FirstName { get; set; }
		[Required(ErrorMessage = "Last name is required")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Occupation is required, if none set 'unemployed' or 'student' ")]
		public string Occupation { get; set; }

		public string Website { get; set; }

		public string about { get; set; }

		public Byte[] ProfileImage { get; set; }

		public bool IsPrivate { get; set; }

		public string email { get; set; }

		[NotMapped]
		public string ProfileImageUrl { get; set; }

		[NotMapped]
		public IdentityUser user { get; set; }

		[NotMapped]
		public IEnumerable<CertificationsModel> Certifications { get; set; }

	}
}
