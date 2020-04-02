using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CertificationsDevelopment.Models {
	public class Fileupload {

		[Key]
		public int FileId { get; set; }
		[Required]
		public int CertificationId { get; set; }
		public string FileName { get; set; }
		public byte[] FileData { get; set; }

		public string Author { get; set; }
		public DateTime DateAdded { get; set; }
	}
}
