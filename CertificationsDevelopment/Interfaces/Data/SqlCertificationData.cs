using CertificationsDevelopment.Data;
using CertificationsDevelopment.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CertificationsDevelopment.Interfaces.Data {
	public class SqlCertificationData : ICertificationData {

		private readonly ApplicationDbContext db;
		public SqlCertificationData(ApplicationDbContext db) {
			this.db = db;
		}

		public CertificationsModel Add(CertificationsModel addCert) {
			db.Add(addCert);
			return addCert;
		}

		public CertificationsModel GetById(int id) {
			return db.Certifications.Find(id);

		}
		public CertificationsModel Delete(int id) {
			var _certification = GetById(id);
			if (_certification != null) {
				db.Certifications.Remove(_certification);
			}
			return _certification;
		}


		public IEnumerable<CertificationsModel> GetCertificationsById(int id) {
			var query = from r in db.Certifications
						where r.Id == id
						select r;
			return query;
		}

		public IEnumerable<CertificationsModel> GetCertificationsByName(string name) {
			var query = from x in db.Certifications
						where x.CertName.ToLower().Contains(name.ToLower()) || string.IsNullOrEmpty(name)
						orderby x.CertName, x.CertSubject, x.CertSite
						select x;
			return query;
		}

		public IEnumerable<CertificationsModel> GetCertificationsBySubject(Subject subject) {
			var query = from x in db.Certifications
						where x.CertSubject.Equals(subject.ToString()) || x.CertSubject.Equals(subject)
						orderby x.CertName
						select x;
			return query;
		}

		public IEnumerable<CertificationsModel> GetCertificationsByAuthor(string Author) {
			var query = from u in db.Certifications
						where u.Author.Equals(Author)
						orderby u.CertSubject, u.CertSite, u.CertName
						select u;
			return query;
		}
		public IEnumerable<CertificationsModel> GetAll() {
			var query = from d in db.Certifications
						select d;
			return query;
		}
		public CertificationsModel Update(CertificationsModel updateCert) {
			var entity = db.Certifications.Attach(updateCert);
			entity.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
			return updateCert;
		}

		public int Commit() {
			try {
				return db.SaveChanges();
			}catch (Exception e) {
				Console.WriteLine($"Error: {e}");
				return 404;
			}
		}

		public int CountCertifications() {
			var count = db.Certifications.Where(x => x.Id != 0).Count();
			return count;
		}

		public IdentityUser GetUser(string email) {
			var query = from x in db.User
						where x.Email == email
						select x;
			return query.FirstOrDefault();
		}
	}
}
