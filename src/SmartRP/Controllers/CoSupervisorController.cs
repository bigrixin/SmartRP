using AutoMapper;
using RazorEngine.Templating;
using SmartRP.Domain;
using SmartRP.Domain.Service;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SmartRP.Controllers
{
	[Authorize]
	public class CoSupervisorController : Controller
	{
		#region Fields

		private readonly IProjectService _projectServices;
		private readonly ICommonService _commonServices;
		private readonly IUserService _userServices;
		private readonly IMapper _mapper;

		#endregion

		#region Ctor

		public CoSupervisorController(IProjectService projectServices, ICommonService commonServices, IUserService userServices, IMapper mapper)
		{
			_projectServices = projectServices;
			_commonServices = commonServices;
			_userServices = userServices;
			_mapper = mapper;
		}
		#endregion

		#region Menu

		[Authorize(Roles = "CoSupervisor")]
		public ActionResult Menu()
		{
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			DynamicViewBag model = _projectServices.GetRequestedCounterModel(currentCoSupervisor.ID, "CoSupervisor");
			return PartialView("_CoSupervisorMenu", model);
		}

		#endregion

		#region Profile

		[HttpGet, Route("cosupervisor/profile")]
		public new ActionResult Profile(int ID)
		{
			ViewBag.TypeOfUser = "CoSupervisor";
			CoSupervisorViewModel model = new CoSupervisorViewModel();
			CoSupervisor currentCoSupervisor = _userServices.GetCurrentCoSupervisor(ID);
			if (currentCoSupervisor == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "CoSupervisor profile error !", errorContent = "Can not find current Co-supervisor!" });
			model = _mapper.Map<CoSupervisor, CoSupervisorViewModel>(currentCoSupervisor);
			model.UserSelectedKeywords = _commonServices.GetUserSelectedKeywordsByUserID(currentCoSupervisor.ID);
			model.EnrolledSubjects = _commonServices.GetEnrolledTermSubjectsVMList(currentCoSupervisor.ID, "CoSupervisor");

			return View(model);
		}

		[Authorize(Roles = "CoSupervisor")]
		public ActionResult MyAccount(string usertype)
		{
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			CoSupervisorViewModel model = new CoSupervisorViewModel();
			if (currentCoSupervisor != null)
			{
				model = _mapper.Map<CoSupervisor, CoSupervisorViewModel>(currentCoSupervisor);
				model.UserSelectedKeywords = _commonServices.GetUserSelectedKeywordsByUserID(currentCoSupervisor.ID);
				model.EnrolledSubjects = _commonServices.GetEnrolledTermSubjectsVMList(currentCoSupervisor.ID, "CoSupervisor");
				Supervisor supervisor = _userServices.GetCurrentSupervisor(currentCoSupervisor.MySupervisorID);
				if (supervisor != null)
					model.MySupervisorName = supervisor.Firstname + " " + supervisor.Lastname;
			}
			return View(model);
		}

		[Authorize(Roles = "CoSupervisor")]
		[HttpGet, Route("coSupervisor/update-profile")]
		public ActionResult UpdateProfile(string userid)
		{
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			CoSupervisorProfileEditModel model = new CoSupervisorProfileEditModel();
			if (currentCoSupervisor != null && currentCoSupervisor.LoginIdentityID == userid)
			{
				model = _mapper.Map<CoSupervisor, CoSupervisorProfileEditModel>(currentCoSupervisor);
				model.KeywordList = _commonServices.GetKeywordList();
				model.SelectedKeywords = _commonServices.GetUserSelectedKeywordsByUserID(currentCoSupervisor.ID);
				model.SupervisorDropDownList = _commonServices.GetSupervisorSelectList();
			}
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Resume", subjectAndUserID = "CoSupervisor" + currentCoSupervisor.ID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");
			return View(model);
		}

		[Authorize(Roles = "CoSupervisor")]
		[HttpPost, Route("coSupervisor/update-profile")]
		public ActionResult UpdateProfile(CoSupervisorProfileEditModel model)
		{
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			if (currentCoSupervisor != null && ModelState.IsValid)
			{
				currentCoSupervisor = _mapper.Map(model, currentCoSupervisor);
				_commonServices.UpdateProfile(currentCoSupervisor, model.SelectedKeywordIDs);
			}
			return RedirectToAction("myaccount");
		}

		#endregion

		#region Subject

		[HttpGet, Route("cosupervisor/enrol-subject")]
		public ActionResult EnrolSubject()
		{
			ViewBag.TypeOfUser = "CoSupervisor";
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			if (currentCoSupervisor == null)
				return View();
			return RedirectToAction("Enrol", "Subjects", new { userID = currentCoSupervisor.LoginIdentityID });
		}

		#endregion

		#region Project - Does not be used in this version

		[Authorize(Roles = "CoSupervisor")]
		[HttpGet, Route("cosupervisor/post-project")]
		public ActionResult PostProject()
		{
			ViewBag.TypeOfUser = "CoSupervisor";
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentCoSupervisorEnrolledSubject(currentCoSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			Project project = new Project(currentCoSupervisor.ID);
			ProjectEditModel model = _projectServices.MapperProjectToEditModel(project, currentCoSupervisor.ID, "CoSupervisor");
			model.SubjectName = currentEnrolledSubject.SubjectName;
			model.ExpiredAt = _commonServices.GetCurrentOpenTerm().EndAt;
			model.CurrentEnrolledSubjectID = currentEnrolledSubject.ID;
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Project", subjectAndUserID = "CoSupervisor" + currentCoSupervisor.ID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");

			return View(model);
		}


		[HttpPost, Route("cosupervisor/post-project")]
		public ActionResult PostProject(ProjectEditModel model)
		{
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			if (currentCoSupervisor != null && ModelState.IsValid)
				_projectServices.AddPostedProject("CoSupervisor", currentCoSupervisor.ID, model);
			return RedirectToAction("my-posted-projects");
		}

		[Authorize(Roles = "CoSupervisor")]
		[HttpGet, Route("cosupervisor/my-posted-projects")]
		public ActionResult MyPostedProjects()
		{
			ViewBag.TypeOfUser = "CoSupervisor";
			return RedirectToAction("my-published-project", "projects");
		}

		#endregion

		#region Published project

		[Authorize(Roles = "CoSupervisor")]
		[HttpGet, Route("cosupervisor/my-published-project")]
		public ActionResult MyPublishedProject()
		{
			ViewBag.TypeOfUser = "CoSupervisor";
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentCoSupervisorEnrolledSubject(currentCoSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current co-supervisoer enrolled subject !" });
			if (currentCoSupervisor.MySupervisorID == 0)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "No supervisor error !", errorContent = "Please setup your supervisor using edit profile !" });
			return RedirectToAction("my-published-project", "projects");
		}


		[Authorize(Roles = "CoSupervisor")]
		[HttpGet, Route("cosupervisor/update-project")]
		public ActionResult UpdateProject(int projectID)
		{
			ViewBag.TypeOfUser = "CoSupervisor";
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentCoSupervisorEnrolledSubject(currentCoSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			Project project = currentCoSupervisor.GetProject(projectID);
			if (project == null)
				return RedirectToAction("my-posted-projects");
			ProjectEditModel model = _projectServices.MapperProjectToEditModel(project, currentCoSupervisor.ID, "CoSupervisor");
			model.MaxGroupNumber = project.GetGroups().Count();
			model.SubjectName = currentEnrolledSubject.SubjectName;
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Project", subjectAndUserID = "CoSupervisor" + currentCoSupervisor.ID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");

			return View(model);
		}


		[HttpPost, Route("cosupervisor/update-project")]
		public ActionResult UpdateProject(ProjectEditModel model)
		{
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			if (currentCoSupervisor != null && ModelState.IsValid)
				_projectServices.UpdatePostedProject(currentCoSupervisor.ID, model);
			return RedirectToAction("my-posted-projects");
		}

		[HttpGet]
		[Authorize(Roles = "CoSupervisor")]
		public ActionResult DeleteProject(int? projectID, int groupID)
		{
			if (projectID == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			ViewBag.TypeOfUser = "CoSupervisor";
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentCoSupervisorEnrolledSubject(currentCoSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			Project project = currentCoSupervisor.GetProject((int)projectID);
			if (project == null)
				return RedirectToAction("my-posted-projects");
			int groupMembers = project.GetGroups().LastOrDefault().GetStudents().Count();
			if (groupMembers > 0)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Delete project error!", errorContent = "Can not delete the project, because someone have joined this group !" });
			var model = _mapper.Map<Project, ProjectEditModel>(project);
			model.KeywordList = _commonServices.GetKeywordList();
			model.UserID = currentCoSupervisor.ID;
			model.PublisherType = "CoSupervisor";
			return View(model);
		}

		// POST: 
		[HttpPost, ActionName("DeleteProject")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int projectID, int groupID)
		{
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			if (currentCoSupervisor != null && ModelState.IsValid)
				_projectServices.DeletePostedProjectGroup(currentCoSupervisor.ID, projectID, groupID);
			return RedirectToAction("my-posted-projects");
		}



		[Authorize(Roles = "CoSupervisor")]
		[HttpGet, Route("cosupervisor/project-list")]
		public ActionResult ProjectList()
		{
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentCoSupervisorEnrolledSubject(currentCoSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current enrolled subject !" });
			if (currentCoSupervisor.MySupervisorID == 0)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "No supervisor error !", errorContent = "Please setup your supervisor using edit profile !" });
			return RedirectToAction("project-list", "projects", new { subjectID = currentEnrolledSubject.ID });
		}

		#endregion

		#region Project Pool

		[HttpGet, Route("cosupervisor/my-project-pool")]
		public ActionResult MyProjectsInPool()
		{
			ViewBag.TypeOfUser = "CoSupervisor";
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentCoSupervisorEnrolledSubject(currentCoSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current co-supervisoer enrolled subject !" });
			if (currentCoSupervisor.MySupervisorID == 0)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "No supervisor error !", errorContent = "Please setup your supervisor using edit profile !" });
			return RedirectToAction("my-project-pool", "projects");
		}


		#endregion

		#region Manage Request 

		[HttpGet, Route("cosupervisor/manage-request")]
		public ActionResult ManageRequest()
		{
			ViewBag.TypeOfUser = "CoSupervisor";
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentCoSupervisorEnrolledSubject(currentCoSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current co-supervisoer enrolled subject !" });
			if (currentCoSupervisor.MySupervisorID == 0)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "No supervisor error !", errorContent = "Please setup your supervisor using edit profile !" });
			return RedirectToAction("manage-request", "projects");
		}

		#endregion

		#region Pick up Project

		[HttpGet, Route("cosupervisor/posted-by-students")]
		public ActionResult PostedByStudents()
		{
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentCoSupervisorEnrolledSubject(currentCoSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current enrolled subject !" });
			if (currentCoSupervisor.MySupervisorID == 0)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "No supervisor error !", errorContent = "Please setup your supervisor using edit profile !" });
			return RedirectToAction("posted-by-students", "projects");
		}

		#endregion

		#region Manage Group

		[HttpGet, Route("cosupervisor/manage-project-group")]
		public ActionResult ManageProjectGroup()
		{
			CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentCoSupervisorEnrolledSubject(currentCoSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current enrolled subject !" });
			if (currentCoSupervisor.MySupervisorID == 0)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "No supervisor error !", errorContent = "Please setup your supervisor using edit profile !" });
			return RedirectToAction("manage-project-group", "groups");
		}

		#endregion

		#region Helper



		#endregion
	}
}