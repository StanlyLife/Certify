using CertificationsDevelopment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertificationsDevelopment.Interfaces {
	public interface IUserProfileData {

		UserProfile GetProfileByID(int id);
		UserProfile GetProfileByUserId(string id);

		UserProfile Add(UserProfile profile);
		UserProfile Update(UserProfile profile);

		UserProfile GetProfileByEmail(string email);
		bool Delete(UserProfile profile);
		bool Commit();

	}
}
