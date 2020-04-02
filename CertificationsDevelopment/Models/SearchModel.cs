using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertificationsDevelopment.Models {
	public class SearchModel {
		public IEnumerable<CertificationsModel> Certifications { get; set; }
		[BindProperty(SupportsGet = true)]

		public UserProfile profile { get; set; }
		public string search { get; set; }
	}
}
