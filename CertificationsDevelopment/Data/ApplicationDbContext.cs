using CertificationsDevelopment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CertificationsDevelopment.Data {
	public class ApplicationDbContext : IdentityDbContext {

		public DbSet<CertificationsModel> Certifications { get; set; }
		public DbSet<Fileupload> Files { get; set; }
		public DbSet<UserProfile> Profile { get; set; }
		public DbSet<IdentityUser> User { get; set; }


		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options) {
			//options.EnableSensitiveDataLogging();
		}
	}
}
