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
	public class ExternalSupervisorController : Controller
	{
		#region Fields

		private readonly IProjectService _projectServices;
		private readonly ICommonService _commonServices;
		private readonly IUserService _userServices;
		private readonly IMapper _mapper;

		#endregion

		#region Menu

		[Authorize(Roles = "ExternalSupervisor")]
		public ActionResult Menu()
		{
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			DynamicViewBag model = _projectServices.GetRequestedCounterModel(currentExternalSupervisor.ID, "ExternalSupervisor");
			return PartialView("_ExtSupervisorMenu", model);
		}

		#endregion

		#region Ctor

		public ExternalSupervisorController(IProjectService projectServices, ICommonService commonServices, IUserService userServices, IMapper mapper)
		{
			_projectServices = projectServices;
			_commonServices = commonServices;
			_userServices = userServices;
			_mapper = mapper;
		}

		#endregion

		#region Profile

		[HttpGet, Route("externalsupervisor/profile")]
		public new ActionResult Profile(int ID)
		{
			ViewBag.TypeOfUser = "ExternalSupervisor";
			ExternalSupervisorViewModel model = new ExternalSupervisorViewModel();
			ExternalSupervisor currentExternalSupervisor = _userServices.GetCurrentExternalSupervisor(ID);
			if (currentExternalSupervisor == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "External Supervisor profile error !", errorContent = "Can not find current external supervisor!" });
			model = _mapper.Map<ExternalSupervisor, ExternalSupervisorViewModel>(currentExternalSupervisor);
			model.UserSelectedKeywords = _commonServices.GetUserSelectedKeywordsByUserID(currentExternalSupervisor.ID);
			model.EnrolledSubjects = _commonServices.GetEnrolledTermSubjectsVMList(currentExternalSupervisor.ID, "ExternalSupervisor");

			return View(model);
		}

		[Authorize(Roles = "ExternalSupervisor")]
		public ActionResult MyAccount()
		{
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			ExternalSupervisorViewModel model = new ExternalSupervisorViewModel();
			if (currentExternalSupervisor != null)
			{
				model = _mapper.Map<ExternalSupervisor, ExternalSupervisorViewModel>(currentExternalSupervisor);
				model.UserSelectedKeywords = _commonServices.GetUserSelectedKeywordsByUserID(currentExternalSupervisor.ID);
				model.EnrolledSubjects = _commonServices.GetEnrolledTermSubjectsVMList(currentExternalSupervisor.ID, "ExternalSupervisor");
				Supervisor supervisor = _userServices.GetCurrentSupervisor(currentExternalSupervisor.MySupervisorID);
				if (supervisor != null)
					model.MySupervisorName = supervisor.Firstname + " " + supervisor.Lastname;
			}
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Resume", subjectAndUserID = "ExtSupervisor" + currentExternalSupervisor.ID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");
			return View(model);
		}

		[Authorize(Roles = "ExternalSupervisor")]
		[HttpGet, Route("externalSupervisor/update-profile")]
		public ActionResult UpdateProfile(string userid)
		{
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			ExternalSupervisorProfileEditModel model = new ExternalSupervisorProfileEditModel();
			if (currentExternalSupervisor != null && currentExternalSupervisor.LoginIdentityID == userid)
			{
				model = _mapper.Map<ExternalSupervisor, ExternalSupervisorProfileEditModel>(currentExternalSupervisor);
				model.KeywordList = _commonServices.GetKeywordList();
				model.SelectedKeywords = _commonServices.GetUserSelectedKeywordsByUserID(currentExternalSupervisor.ID);
				model.SupervisorDropDownList = _commonServices.GetSupervisorSelectList();
			}
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Resume", subjectAndUserID = "ExtSupervisor" + currentExternalSupervisor.ID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");
			return View(model);
		}

		[Authorize(Roles = "ExternalSupervisor")]
		[HttpPost, Route("externalSupervisor/update-profile")]
		public ActionResult UpdateProfile(ExternalSupervisorProfileEditModel model)
		{
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			if (currentExternalSupervisor != null && ModelState.IsValid)
			{
				currentExternalSupervisor = _mapper.Map(model, currentExternalSupervisor);
				_commonServices.UpdateProfile(currentExternalSupervisor, model.SelectedKeywordIDs);
			}
			return RedirectToAction("myaccount");
		}

		#endregion

		#region Subject

		[HttpGet, Route("externalsupervisor/enrol-subject")]
		public ActionResult EnrolSubject()
		{
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			if (currentExternalSupervisor == null)
				return View();
			return RedirectToAction("Enrol", "Subjects", new { userID = currentExternalSupervisor.LoginIdentityID });
		}

		#endregion

		#region Project - Does not be used in this version

		[Authorize(Roles = "ExternalSupervisor")]
		[HttpGet, Route("externalsupervisor/project-list")]
		public ActionResult ProjectList()
		{
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentExternalSupervisorEnrolledSubject(currentExternalSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current enrolled subject !" });
			if (currentExternalSupervisor.MySupervisorID == 0)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "No supervisor error !", errorContent = "Please setup your supervisor using edit profile !" });
			return RedirectToAction("project-list", "projects", new { subjectID = currentEnrolledSubject.ID });
		}


		[Authorize(Roles = "ExternalSupervisor")]
		[HttpGet, Route("externalsupervisor/post-project")]
		public ActionResult PostProject()
		{
			ViewBag.TypeOfUser = "ExternalSupervisor";
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentExternalSupervisorEnrolledSubject(currentExternalSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			Project project = new Project(currentExternalSupervisor.ID);
			ProjectEditModel model = _projectServices.MapperProjectToEditModel(project, currentExternalSupervisor.ID, "ExternalSupervisor");
			model.SubjectName = currentEnrolledSubject.SubjectName;
			model.ExpiredAt = _commonServices.GetCurrentOpenTerm().EndAt;
			model.CurrentEnrolledSubjectID = currentEnrolledSubject.ID;
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Project", subjectAndUserID = "ExtSupervisor" + currentExternalSupervisor.ID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");

			return View(model);
		}


		[HttpPost, Route("externalsupervisor/post-project")]
		public ActionResult PostProject(ProjectEditModel model)
		{
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			if (currentExternalSupervisor != null && ModelState.IsValid)
				_projectServices.AddPostedProject("ExternalSupervisor", currentExternalSupervisor.ID, model);
			return RedirectToAction("my-posted-projects");
		}

		[Authorize(Roles = "ExternalSupervisor")]
		[HttpGet, Route("externalsupervisor/my-posted-projects")]
		public ActionResult MyPostedProjects()
		{
			ViewBag.TypeOfUser = "ExternalSupervisor";
			return RedirectToAction("my-published-project", "projects");
		}

		#endregion

		#region Published project

		[Authorize(Roles = "ExternalSupervisor")]
		[HttpGet, Route("externalsupervisor/my-published-project")]
		public ActionResult MyPublishedProject()
		{
			ViewBag.TypeOfUser = "ExternalSupervisor";
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentExternalSupervisorEnrolledSubject(currentExternalSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current co-supervisoer enrolled subject !" });
			if (currentExternalSupervisor.MySupervisorID == 0)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "No supervisor error !", errorContent = "Please setup your supervisor using edit profile !" });
			return RedirectToAction("my-published-project", "projects");
		}


		[Authorize(Roles = "ExternalSupervisor")]
		[HttpGet, Route("externalsupervisor/update-project")]
		public ActionResult UpdateProject(int projectID)
		{
			ViewBag.TypeOfUser = "ExternalSupervisor";
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentExternalSupervisorEnrolledSubject(currentExternalSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			Project project = currentExternalSupervisor.GetProject(projectID);
			if (project == null)
				return RedirectToAction("my-posted-projects");
			ProjectEditModel model = _projectServices.MapperProjectToEditModel(project, currentExternalSupervisor.ID, "ExternalSupervisor");
			model.SubjectName = currentEnrolledSubject.SubjectName;
			model.MaxGroupNumber = project.GetGroups().Count();
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Project", subjectAndUserID = "ExtSupervisor" + currentExternalSupervisor.ID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");

			return View(model);
		}


		[HttpPost, Route("externalsupervisor/update-project")]
		public ActionResult UpdateProject(ProjectEditModel model)
		{
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			if (currentExternalSupervisor != null && ModelState.IsValid)
				_projectServices.UpdatePostedProject(currentExternalSupervisor.ID, model);
			return RedirectToAction("my-posted-projects");
		}

		[HttpGet]
		[Authorize(Roles = "ExternalSupervisor")]
		public ActionResult DeleteProject(int? projectID, int groupID)
		{
			if (projectID == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			ViewBag.TypeOfUser = "ExternalSupervisor";
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentExternalSupervisorEnrolledSubject(currentExternalSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			Project project = currentExternalSupervisor.GetProject((int)projectID);
			if (project == null)
				return RedirectToAction("my-posted-projects");
			int groupMembers = project.GetGroups().LastOrDefault().GetStudents().Count();
			if (groupMembers > 0)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Delete project error!", errorContent = "Can not delete the project, because someone have joined this group !" });
			var model = _mapper.Map<Project, ProjectEditModel>(project);
			model.KeywordList = _commonServices.GetKeywordList();
			model.UserID = currentExternalSupervisor.ID;
			model.PublisherType = "ExternalSupervisor";
			return View(model);
		}

		// POST: 
		[HttpPost, ActionName("DeleteProject")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int projectID, int groupID)
		{
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			if (currentExternalSupervisor != null && ModelState.IsValid)
				_projectServices.DeletePostedProjectGroup(currentExternalSupervisor.ID, projectID, groupID);
			return RedirectToAction("my-posted-projects");
		}



		#endregion

		#region Project Pool

		[HttpGet, Route("externalsupervisor/my-project-pool")]
		public ActionResult MyProjectsInPool()
		{
			ViewBag.TypeOfUser = "ExternalSupervisor";
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentExternalSupervisorEnrolledSubject(currentExternalSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current co-supervisoer enrolled subject !" });
			if (currentExternalSupervisor.MySupervisorID == 0)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "No supervisor error !", errorContent = "Please setup your supervisor using edit profile !" });
			return RedirectToAction("my-project-pool", "projects");
		}


		#endregion

		#region Manage Request 

		[HttpGet, Route("externalsupervisor/manage-request")]
		public ActionResult ManageRequest()
		{
			ViewBag.TypeOfUser = "ExternalSupervisor";
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentExternalSupervisorEnrolledSubject(currentExternalSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current co-supervisoer enrolled subject !" });
			if (currentExternalSupervisor.MySupervisorID == 0)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "No supervisor error !", errorContent = "Please setup your supervisor using edit profile !" });
			return RedirectToAction("manage-request", "projects");
		}

		#endregion

		#region Pick up Project

		[HttpGet, Route("externalsupervisor/posted-by-students")]
		public ActionResult PostedByStudents()
		{
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentExternalSupervisorEnrolledSubject(currentExternalSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current enrolled subject !" });
			if (currentExternalSupervisor.MySupervisorID == 0)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "No supervisor error !", errorContent = "Please setup your supervisor using edit profile !" });
			return RedirectToAction("posted-by-students", "projects");
		}

		#endregion

		#region Manage Group

		[HttpGet, Route("externalsupervisor/manage-project-group")]
		public ActionResult ManageProjectGroup()
		{
			ExternalSupervisor currentExternalSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
			Subject currentEnrolledSubject = _commonServices.GetCurrentExternalSupervisorEnrolledSubject(currentExternalSupervisor);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current enrolled subject !" });
			if (currentExternalSupervisor.MySupervisorID == 0)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "No supervisor error !", errorContent = "Please setup your supervisor using edit profile !" });
			return RedirectToAction("manage-project-group", "groups");
		}

		#endregion


		#region Helper



		#endregion
	}
}