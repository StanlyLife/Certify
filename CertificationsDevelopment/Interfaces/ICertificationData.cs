using CertificationsDevelopment.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using static CertificationsDevelopment.Models.CertificationsModel;

namespace CertificationsDevelopment.Interfaces {
	public interface ICertificationData {
		IEnumerable<CertificationsModel> GetCertificationsByName(string name);
		IEnumerable<CertificationsModel> GetCertificationsBySubject(Subject subject);
		IEnumerable<CertificationsModel> GetCertificationsById(int id);
		IEnumerable<CertificationsModel> GetCertificationsByAuthor(string Author);

		public int Commit();
		public int CountCertifications();

		IdentityUser GetUser(string email);
		CertificationsModel GetById(int id);
		CertificationsModel Update(CertificationsModel updateCert);
		CertificationsModel Add(CertificationsModel addCert);
		CertificationsModel Delete(int id);
		IEnumerable<CertificationsModel> GetAll();
	}
}
