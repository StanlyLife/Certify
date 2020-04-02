using CertificationsDevelopment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertificationsDevelopment.Interfaces {
	public interface IFileUploadData {

		Fileupload GetByFileId(int id);
		IEnumerable<Fileupload> GetByCertId(int id);
		Fileupload Add(Fileupload file);
		Fileupload Delete(int CertificationId);
		Fileupload Update(Fileupload file);
		int Commit();
		bool DoesFileExist(Fileupload file);

	}
}
