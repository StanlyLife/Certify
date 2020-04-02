using CertificationsDevelopment.Interfaces;
using CertificationsDevelopment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CertificationsDevelopment.Controllers {
	public class HomeController : Controller {
		private readonly ILogger<HomeController> _logger;
		private readonly ICertificationData certData;

		public HomeController(ILogger<HomeController> logger, ICertificationData certData) {
			_logger = logger;
			this.certData = certData;
		}

		public ActionResult Index() {
			CountModel counter = new CountModel();
			counter.certificationAmount = certData.CountCertifications();
			return View(counter);
		}

		public IActionResult Privacy() {
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error() {
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}


	}
}
