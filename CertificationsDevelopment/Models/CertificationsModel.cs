using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CertificationsDevelopment.Models {
	public class CertificationsModel {

		public int Id { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be under 25 and more than 3 characters")]
		public string CertName { get; set; }
		public Subject CertSubject { get; set; }
		[StringLength(250)]
		public string CertDescription { get; set; }

		[StringLength(256, ErrorMessage = "URL must be entered")]
		public string CertUrl { get; set; }
		public Site CertSite { get; set; }
		public DateTime Posted { get; set; }
                              
		public string Author { get; set; }



	}
}
