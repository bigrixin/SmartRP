using AutoMapper;
using RazorEngine.Templating;
using SmartRP.Domain;
using SmartRP.Domain.Service;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SmartRP.Controllers
{
	[Authorize]
	public class SupervisorController : Controller
	{
		#region Fields

		private readonly IProjectService _projectServices;
		private readonly ICommonService _commonServices;
		private readonly IUserService _userServices;
		private readonly IMapper _mapper;

		#endregion

		#region Ctor

		public SupervisorController(IProjectService projectServices, ICommonService commonServices, IUserService userServices, IMapper mapper)
		{
			_projectServices = projectServices;
			_commonServices = commonServices;
			_userServices = userServices;
			_mapper = mapper;
		}

		#endregion

		#region Menu

		[Authorize(Roles = "Supervisor")]
		public ActionResult Menu()
		{
			Supervisor currentSupervisor = this.GetLoggedInUser() as Supervisor;
			DynamicViewBag model = _projectServices.GetRequestedCounterModel(currentSupervisor.ID, "Supervisor");
			return PartialView("_SupervisorMenu", model);
		}

		#endregion

		#region Profile

		[Authorize(Roles = "Supervisor")]
		public ActionResult MyAccount()
		{
			Supervisor currentSupervisor = this.GetLoggedInUser() as Supervisor;
			SupervisorViewModel model = new SupervisorViewModel();
			if (currentSupervisor != null)
			{
				model = _mapper.Map<Supervisor, SupervisorViewModel>(currentSupervisor);
				model.UserSelectedKeywords = _commonServices.GetUserSelectedKeywordsByUserID(currentSupervisor.ID);
			}
			return View(model);
		}

		[Authorize(Roles = "Supervisor")]
		[HttpGet, Route("supervisor/update-profile")]
		public ActionResult UpdateProfile(string userid)
		{
			Supervisor currentSupervisor = this.GetLoggedInUser() as Supervisor;
			SupervisorProfileEditModel model = new SupervisorProfileEditModel();
			if (currentSupervisor != null && currentSupervisor.LoginIdentityID == userid)
			{
				model = _mapper.Map<Supervisor, SupervisorProfileEditModel>(currentSupervisor);
				model.KeywordList = _commonServices.GetKeywordList();
				model.SelectedKeywords = _commonServices.GetUserSelectedKeywordsByUserID(currentSupervisor.ID);
			}
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Resume", subjectAndUserID = "Supervisor" + currentSupervisor.ID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");
			return View(model);
		}

		[Authorize(Roles = "Supervisor")]
		[HttpPost, Route("supervisor/update-profile")]
		public ActionResult UpdateProfile(SupervisorProfileEditModel model)
		{
			Supervisor currentSupervisor = this.GetLoggedInUser() as Supervisor;
			if (currentSupervisor != null && ModelState.IsValid)
			{
				currentSupervisor = _mapper.Map(model, currentSupervisor);
				_commonServices.UpdateProfile(currentSupervisor, model.SelectedKeywordIDs);
			}
			return RedirectToAction("myaccount");
		}

		[HttpGet, Route("supervisor/profile")]
		public new ActionResult Profile(int ID)
		{
			ViewBag.TypeOfUser = "Supervisor";
			SupervisorViewModel model = new SupervisorViewModel();
			Supervisor currentSupervisor = _userServices.GetCurrentSupervisor(ID);
			if (currentSupervisor == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Supervisor profile error !", errorContent = "Can not find current supervisor!" });

			model = _mapper.Map<Supervisor, SupervisorViewModel>(currentSupervisor);
			model.UserSelectedKeywords = _commonServices.GetUserSelectedKeywordsByUserID(currentSupervisor.ID);

			return View(model);
		}

		#endregion

		#region Project - does not use in current version

		[Authorize(Roles = "Supervisor")]
		[HttpGet, Route("supervisor/post-project")]
		public ActionResult PostProject()
		{
			//not use
			Supervisor currentSupervisor = this.GetLoggedInUser() as Supervisor;
			if (currentSupervisor == null)
				return View();
			Term currentTerm = _commonServices.GetCurrentOpenTerm();
			if (currentTerm == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Semester not open!", errorContent = "Can not find current open semester !" });
			Project project = new Project(currentSupervisor.ID);
			ProjectEditModel model = _projectServices.MapperProjectToEditModel(project, currentSupervisor.ID, "Supervisor");
			model.ExpiredAt = currentTerm.EndAt;
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Project", subjectAndUserID = "Supervisor" + currentSupervisor.ID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");

			return View(model);
		}


		[HttpPost, Route("supervisor/post-project")]
		public ActionResult PostProject(ProjectEditModel model)
		{
			//not use
			Supervisor currentSupervisor = this.GetLoggedInUser() as Supervisor;
			if (currentSupervisor != null && ModelState.IsValid)
			{
				if (!_projectServices.AddPostedProjectBySupervisor(currentSupervisor, model))
					return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Supervisor post project error !", errorContent = "Can not post a project in the subject !" });
			}
			else
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Supervisor post project model error !", errorContent = "Can not post a project in the subject !" });

			return RedirectToAction("my-posted-projects");
		}

		[Authorize(Roles = "Supervisor")]
		[HttpGet, Route("supervisor/my-posted-projects")]
		public ActionResult MyPostedProjects()
		{
			ViewBag.TypeOfUser = "Supervisor";
			return RedirectToAction("my-published-project", "projects");
		}

		#endregion


		#region Project

		[Authorize(Roles = "Supervisor")]
		[HttpGet, Route("supervisor/update-project")]
		public ActionResult UpdateProject(int projectID)
		{
			Supervisor currentSupervisor = this.GetLoggedInUser() as Supervisor;
			if (currentSupervisor == null)
				return View();
			Term currentTerm = _commonServices.GetCurrentOpenTerm();
			if (currentTerm == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Semester not open!", errorContent = "Can not find current open semester !" });

			Project project = currentSupervisor.GetProject(projectID);
			if (project == null)
				return RedirectToAction("my-posted-projects");
			ProjectEditModel model = _projectServices.MapperProjectToEditModel(project, currentSupervisor.ID, "Supervisor");
			model.MaxGroupNumber= project.GetGroups().Count();
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Project", subjectAndUserID = "Supervisor" + currentSupervisor.ID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");

			return View(model);
		}


		[HttpPost, Route("supervisor/update-project")]
		public ActionResult UpdateProject(ProjectEditModel model)
		{
			Supervisor currentSupervisor = this.GetLoggedInUser() as Supervisor;
			if (currentSupervisor != null && ModelState.IsValid)
			{
				if (!_projectServices.UpdatePostedProjectBySupervisor(currentSupervisor.ID, model))
					return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Supervisor update project error !", errorContent = "Can not update current project!" });
			}
			else
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Supervisor update project model error !", errorContent = "Can not update a project in the subject !" });

			return RedirectToAction("my-posted-projects");
		}

		[HttpGet]
		[Authorize(Roles = "Supervisor")]
		public ActionResult DeleteProject(int? projectID, int groupID)
		{
			if (projectID == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			Supervisor currentSupervisor = this.GetLoggedInUser() as Supervisor;
			if (currentSupervisor == null)
				return View();
			Term currentTerm = _commonServices.GetCurrentOpenTerm();
			if (currentTerm == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Semester not open!", errorContent = "Can not find current open semester !" });

			Project project = currentSupervisor.GetProject((int)projectID);
			if (project == null)
				return RedirectToAction("my-posted-projects");

			var groups = project.GetGroups();
			if (groups.Count() != 0)
			{
				int groupMembers = groups.LastOrDefault().GetStudents().Count();
				if (groupMembers > 0)
					return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Delete project error!", errorContent = "Can not delete the project, because someone have joined this group !" });
			}
			var model = _mapper.Map<Project, ProjectEditModel>(project);
			model.KeywordList = _commonServices.GetKeywordList();
			model.UserID = currentSupervisor.ID;
			model.PublisherType = "Supervisor";

			return View(model);
		}

		// POST: 
		[HttpPost, ActionName("DeleteProject")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int projectID, int groupID)
		{
			Supervisor currentSupervisor = this.GetLoggedInUser() as Supervisor;
			if (currentSupervisor != null && ModelState.IsValid)
				_projectServices.DeletePostedProjectGroup(currentSupervisor.ID, projectID, groupID);

			return RedirectToAction("my-posted-projects");
		}

		#endregion

		#region List-Request-Pick up

		[Authorize(Roles = "Supervisor")]
		[HttpGet, Route("supervisor/project-list")]
		public ActionResult ProjectList()
		{
			//supervisor does not need to enrol a subjects
			return RedirectToAction("project-list", "projects", new { subjectID = 0 });
		}

		[HttpGet, Route("supervisor/manage-request")]
		public ActionResult ManageRequest()
		{
			return RedirectToAction("manage-request", "projects");
		}

		[HttpGet, Route("supervisor/posted-by-students")]
		public ActionResult PostedByStudents()
		{
			return RedirectToAction("posted-by-students", "projects");
		}

		#endregion


		#region my co-supervisor


		[HttpGet, Route("supervisor/my-co-supervisor")]
		public ActionResult MyCoSupervisor()
		{
			Supervisor currentSupervisor = this.GetLoggedInUser() as Supervisor;
			if (currentSupervisor == null)
				return View();
			ViewBag.TypeOfUser = "Supervisor";
			List<MyCoSupervisorViewModel> myCoSupervisorVMList = _projectServices.GetMyCoSupervisorVMList(currentSupervisor.ID);
			return View(myCoSupervisorVMList);
		}

		[HttpGet, Route("supervisor/my-co-supervisor-project-pool")]
		public ActionResult MyCoSupervisorProjectPool(int cosupervisorID)
		{
			Supervisor currentSupervisor = this.GetLoggedInUser() as Supervisor;
			if (currentSupervisor == null)
				return View();
			ViewBag.TypeOfUser = "Supervisor";
			List<ProjectPoolEditModel> projectPoolsVM = _projectServices.GetMyCoSupervisorProjectPoolVMList(cosupervisorID);
			return View(projectPoolsVM);
		}

		[HttpGet, Route("supervisor/my-co-supervisor-published-projects")]
		public ActionResult MyCoSupervisorPublishedProjects(int cosupervisorID)
		{
			Supervisor currentSupervisor = this.GetLoggedInUser() as Supervisor;
			if (currentSupervisor == null)
				return View();

			ViewBag.TypeOfUser = "Supervisor";
			List<ProjectViewModel> projectsVM = _projectServices.GetMyCoSupervisorPublishedPorjectsVMList(cosupervisorID);
			return View(projectsVM);
		}


		#endregion
	}
}