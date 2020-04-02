using CertificationsDevelopment.Data;
using CertificationsDevelopment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertificationsDevelopment.Interfaces.Data {
	public class SqlUserProfileData : IUserProfileData {
		private readonly ApplicationDbContext db;

		public SqlUserProfileData(ApplicationDbContext db) {
			this.db = db;
		}
		public UserProfile GetProfileByID(int id) {

			var query = from q in db.Profile
						where q.ProfileId == id
						select q;
			if (query.Any()) {
				return query.FirstOrDefault();
			}
			Console.WriteLine("Something went wrong, not able to find user with ID " + id);
			return new UserProfile();
		}

		public UserProfile GetProfileByUserId(string id) {
			var query = from q in db.Profile
						where q.UserKey == id
						select q;
			return query.FirstOrDefault();
		}

		public  UserProfile Update(UserProfile profile) {
			
			var entity = db.Profile.Attach(profile);
			entity.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
			return profile;
		}

		public UserProfile Add(UserProfile profile) {
			db.Profile.Add(profile);
			return profile;
		}

		public UserProfile GetProfileByEmail(string email) {
			var query = from q in db.Profile
						where q.email.ToLower() == email.ToLower()
						select q;
			if (query.Any()) {
				return query.FirstOrDefault();
			}
			return new UserProfile();
		}

		public bool Commit() {
			if (db.SaveChanges() > 0) {
			return true;
			}
			return false;

		}

		public bool Delete(UserProfile profile) {
			var _file = GetProfileByID(profile.ProfileId);
			if (_file != null) {
				db.Profile.Remove(_file);
				return true;
			}
			return false;
			
		}

	}
}
