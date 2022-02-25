using AutoMapper;
using System.Net;
using System.Web.Mvc;
using SmartRP.Domain;
using SmartRP.Domain.Service;

namespace SmartRP.Controllers
{
	public class SubjectsController : Controller
	{

		#region Fields


		private readonly IUserService _userServices;
		private readonly ICommonService _commonServices;
		private readonly IMapper _mapper;

		#endregion

		#region Ctor

		public SubjectsController(IUserService userServices, ICommonService commonServices,  IMapper mapper)
		{
			_userServices = userServices;
			_commonServices = commonServices;
			_mapper = mapper;
		}

		#endregion

		#region Action

		[HttpGet]
		public ActionResult Enrol(string userID)
		{
			if (userID == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			if (_userServices.IsNewUser(User.Identity.Name))
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "User profile error", errorContent = "Please fill in profile first!" });

			ViewBag.TypeOfUser = _userServices.GetCurrentRole(User.Identity.Name);
			EnrolSubjectViewModel model = new EnrolSubjectViewModel();
			Term term = _commonServices.GetCurrentOpenTerm();
			if (term == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Enrol subject error", errorContent = "Can not find current open subject!" });

			model = _mapper.Map<Term, EnrolSubjectViewModel>(term);
			model.UserID = userID;
			return View(model);
		}

		[HttpPost]
		public ActionResult Enrol(EnrolSubjectViewModel model)
		{
			var userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			if (_commonServices.UserEnrolSubject(userType, model)==null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Enrol subject error", errorContent = "Find a subject has enrolled in the current semester or subject find error!" });

			return RedirectToAction("index", "myaccount");
		}

		#endregion

	}
}
