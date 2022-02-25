using System.Web.Mvc;
using SmartRP.Domain;
using SmartRP.Infrastructure.Data;
using SmartRP.Domain.Service;
using AutoMapper;
using System;
using System.Collections.Generic;
using PagedList;
using System.Linq;

namespace SmartRP.Controllers
{
	[Authorize]
	public class ProjectsController : Controller
	{

		#region Fields

		private readonly ICommonService _commonServices;
		private readonly IProjectService _projectServices;
		private readonly IUserService _userServices;
		private readonly IUploadService _uploadServices;
		private readonly DbData _dbData;
		private readonly IMapper _mapper;
		private const int PageSize = 10;

		#endregion

		#region Ctor

		public ProjectsController(ICommonService commonServices, IProjectService projectServices, IUserService userServices, IUploadService uploadServices, DbData dbData, IMapper mapper)
		{
			_commonServices = commonServices;
			_projectServices = projectServices;
			_userServices = userServices;
			_uploadServices = uploadServices;
			_dbData = dbData;
			_mapper = mapper;
		}

		#endregion

		#region ProjectPool

		[Authorize(Roles = "Supervisor, CoSupervisor, ExternalSupervisor")]
		[HttpGet, Route("projects/my-project-pool")]
		public ActionResult MyProjectsInPool()
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			List<ProjectPoolEditModel> projectPoolsVM = getMyProjectPoolVMList(userType);

			return View(projectPoolsVM);
		}

		[Authorize(Roles = "Supervisor, CoSupervisor, ExternalSupervisor")]
		[HttpGet, Route("projects/add-project-to-pool")]
		public ActionResult AddProjectToPool()
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			User user = _userServices.GetUserIDByEmail(User.Identity.Name);
			ProjectPool projectPool = new ProjectPool(user.ID);
			ProjectPoolEditModel model = _mapper.Map<ProjectPool, ProjectPoolEditModel>(projectPool);
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "ProjectPool", subjectAndUserID = "userID-" + user.ID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");

			return View(model);
		}

		[HttpPost, Route("projects/add-project-to-pool")]
		public ActionResult AddProjectToPool(ProjectPoolEditModel model)
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			User user = _userServices.GetUserIDByEmail(User.Identity.Name);

			if (user != null && ModelState.IsValid)
			{
				if (!_projectServices.AddProjectToPool(userType, user.ID, model))
					return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "post projectPool error !", errorContent = "Can not post a projectPool in the subject !" });
			}
			else
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "post projectPool model error !", errorContent = "Can not post a projectPool in the subject !" });

			return RedirectToAction("my-project-pool");
		}


		[Authorize(Roles = "Supervisor, CoSupervisor, ExternalSupervisor")]
		[HttpGet, Route("projects/update-project-of-pool")]
		public ActionResult UpdateProjectOfPool(int projectID)
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			User user = _userServices.GetUserIDByEmail(User.Identity.Name);
			ProjectPool projectPool = getProjectPoolByProjectID(userType, projectID);
			if (projectPool == null)
				return RedirectToAction("my-project-pool");
			ProjectPoolEditModel model = _mapper.Map<ProjectPool, ProjectPoolEditModel>(projectPool);
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "ProjectPool", subjectAndUserID =  "userID-" + user.ID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");
			return View(model);
		}


		[HttpPost, Route("projects/update-project-of-pool")]
		public ActionResult UpdateProjectOfPool(ProjectPoolEditModel model)
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			User user = _userServices.GetUserIDByEmail(User.Identity.Name);

			if (user != null && ModelState.IsValid)
			{
				if (!_projectServices.UpdateProjectOfPool(userType, user.ID, model))
					return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "update projectPool error !", errorContent = "Can not update current projectPool!" });
			}
			else
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "update projectPool model error !", errorContent = "Can not update a projectPool in the subject !" });

			return RedirectToAction("my-project-pool");
		}

		[HttpGet]
		[Authorize(Roles = "Supervisor, CoSupervisor, ExternalSupervisor")]
		public ActionResult DeleteProjectInPool(int? projectID)
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;

			ProjectPool projectPool = getProjectPoolByProjectID(userType, (int)projectID);
			if (projectPool == null)
				return RedirectToAction("my-project-pool");

			var model = _mapper.Map<ProjectPool, ProjectPoolEditModel>(projectPool);
			return View(model);
		}

		// POST: 
		[HttpPost, ActionName("DeleteProjectInPool")]
		[ValidateAntiForgeryToken]
		public ActionResult DeletePoolProjectConfirmed(int projectID)
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			User user = _userServices.GetUserIDByEmail(User.Identity.Name);

			if (user != null && ModelState.IsValid)
				_projectServices.DeletePostedProjectPool(userType, user.ID, projectID);
			return RedirectToAction("my-project-pool");
		}

		#endregion

		#region Publish - PoolProject to Project

		[HttpGet, Route("projects/publish-project")]
		[Authorize(Roles = "Supervisor, CoSupervisor, ExternalSupervisor")]
		public ActionResult PublishProject(int projectID)
		{
			//the projectID is the projectPoolID
			Term currentTerm = _commonServices.GetCurrentOpenTerm();
			if (currentTerm == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Semester not open!", errorContent = "Can not find current open semester !" });
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			User user = _userServices.GetUserIDByEmail(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			ProjectPool projectPool = getProjectPoolByProjectID(userType, projectID);
			if (projectPool == null)
				return RedirectToAction("my-project-pool");
			Project project = _mapper.Map<ProjectPool, Project>(projectPool);
			project.ID = 0;
			project.ExpiredAt = currentTerm.EndAt;
			project.PublisherType = userType;
			project.Status = ProjectStatus.Opening;
			ProjectEditModel model = _projectServices.MapperProjectToEditModel(project, user.ID, userType);
			Subject subject = getUserRegisteredCurrentSubject(userType);

			///may need add: copy document from \projectpool to \project and change DocumentURL
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Project", subjectAndUserID = "userID-" + user.ID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");

			return View(model);
		}

		[HttpPost, Route("projects/publish-project")]
		public ActionResult PublishProject(ProjectEditModel model)
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			User user = _userServices.GetUserIDByEmail(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			if (ModelState.IsValid)
			{
				Project project = user.GetProjects().Where(p => p.Title == model.Title && p.SubjectID == model.SubjectID).FirstOrDefault();
				if (project != null)
					return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Publish project error !", errorContent = "Can not publish two projects with the same title during the same subject !" });

				if (!_projectServices.AddPostedProject(userType, user.ID, model))
					return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Publish project error !", errorContent = "Can not publish this project in the subject !" });
			}
			else
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Publish project model state error !", errorContent = "Can not post a project in the subject !" });

			return RedirectToAction("my-published-project");
		}

		#endregion

		#region List  

		[HttpGet, Route("projects/my-published-project")]
		public ActionResult MyPublishedProject()
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			User user = _userServices.GetUserIDByEmail(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			List<ProjectViewModel> projectsVM = getMyProjectVMList(userType);

			return View(projectsVM);
		}

		[Authorize(Roles = "Student, Supervisor, CoSupervisor, Coordinator")]
		[HttpGet, Route("projects/project-list")]
		public ActionResult ProjectsList(int? subjectID, string sortOrder, string currentFilter, string searchString, int? page)
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);

			ViewBag.CurrentSort = sortOrder;
			ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
			ViewBag.StatusSortParm = sortOrder == "status" ? "statusdesc" : "status";
			ViewBag.IDSortParm = sortOrder == "id" ? "iddesc" : "id";
			if (searchString != null)
				page = 1;
			else
				searchString = currentFilter;

			ViewBag.CurrentFilter = searchString;
			int pageIndex = 1;
			pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
			IEnumerable<Project> projects;
			if (userType == "Supervisor" || userType == "Coordinator")
			{
				projects = _projectServices.GetCurrentSemesterProjectList();
			}
			else if (userType == "CoSupervisor")
			{
				CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
				projects = _projectServices.GetCurrentEnrolledSemesterByCoSupervisorProjectList(currentCoSupervisor.ID);
			}
			else
			{
				if (subjectID == null)
					return RedirectToAction("project-list", userType);
				projects = _projectServices.GetCurrentSubjectProjectList((int)subjectID);
			}

			if (projects == null)
				return RedirectToAction("myaccount", userType);
			var projectsVM = new List<ProjectViewModel>();

			if (!String.IsNullOrEmpty(searchString))
			{
				projects = projects.Where(s => s.Title.ToLower().Contains(searchString.ToLower())
									|| s.Description.ToLower().Contains(searchString.ToLower())
								 	|| s.SkillsRequest.ToLower().Contains(searchString.ToLower()));

			}

			switch (sortOrder)
			{
				case "title_desc":
					projects = projects.OrderByDescending(s => s.Title);
					break;
				case "status":
					projects = projects.OrderBy(s => s.Status);
					break;
				case "statusdesc":
					projects = projects.OrderByDescending(s => s.Status);
					break;
				case "id":
					projects = projects.OrderBy(s => s.ID);
					break;
				case "iddesc":
					projects = projects.OrderByDescending(s => s.ID);
					break;
				default:  // Name ascending 
					projects = projects.OrderByDescending(s => s.ID);
					break;
			}
			projects.ToList().ForEach(p =>
			{
				ProjectViewModel model = _mapper.Map<Project, ProjectViewModel>(p);
				model.ProjectGroups = p.GetGroups();
				projectsVM.Add(model);
			});

			int pageNumber = (page ?? 1);

			ViewBag.SubjectID = subjectID;
			ViewBag.TypeOfUser = userType;
			if (userType == "Student")
			{
				Student currentStudent = this.GetLoggedInUser() as Student;
				ViewBag.HasJoinedGroup = currentStudent.HasJoinedCurrentSubjectProjectGroup;
			}

			ViewBag.UserID = _userServices.GetUserIDByEmail(User.Identity.Name).ID;

			return View(projectsVM.ToPagedList(pageNumber, PageSize));
		}

		#endregion

		#region Request 

		[HttpGet, Route("projects/manage-request")]
		public ActionResult ManageRequest()
		{
			Term currentTerm = _commonServices.GetCurrentOpenTerm();
			if (currentTerm == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Semester not open!", errorContent = "Can not find current open semester !" });
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			User user = _userServices.GetUserIDByEmail(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			var requestedListVM = _projectServices.GetCurrentTermMyPostedProjectJoinGroupRequestVMList(currentTerm.ID, user.ID);
			return View(requestedListVM);
		}

		[HttpGet, Route("projects/process-request")]
		public ActionResult ProcessRequest(int currentUserID, int studentID, int projectID, int groupID, string actionWord)
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;

			var result = _projectServices.ProcessStudentJoinGroupRequirement(currentUserID, studentID, projectID, groupID, actionWord);
			if (!result)
			{
				if (actionWord == "register")
					return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Process request error", errorContent = actionWord + " action error, the project group may closed. click project title to check details !" });
				else
					return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Process request error", errorContent = actionWord + " action error !" });
			}

			if (userType == "Student")
			{
				if (actionWord == "register")
					return RedirectToAction("my-project-group", userType);
				else if (actionWord == "accept" || actionWord == "reject")
					return RedirectToAction("manage-request", userType);
				else
					return RedirectToAction("my-requested-list", userType);
			}
			else
				return RedirectToAction("manage-request", userType);
		}


		#endregion


		#region Details

		[HttpGet, Route("projects/pool-project-details")]
		[Authorize(Roles = "Supervisor, CoSupervisor, ExternalSupervisor, Coordinator")]
		public ActionResult PoolProjectDetails(string userType, int userID, int projectID)
		{
			ProjectPoolEditModel model = _projectServices.MapperProjectPoolToViewModel(userType, userID, projectID);
			return View(model);
		}

		[HttpGet]
		public ActionResult Details(int projectID, string keyword)
		{
			ProjectViewModel model = _projectServices.MapperProjectToViewModel(projectID);
			ViewBag.Keyword = keyword;
			return View(model);
		}

		#endregion

		#region For Student

		[Authorize(Roles = "Supervisor, CoSupervisor, Coordinator")]
		[HttpGet, Route("projects/posted-by-students")]
		public ActionResult PostedByStudents()
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			List<ProjectViewModel> projectsVM = new List<ProjectViewModel>();
			Subject subject = getUserRegisteredCurrentSubject(userType);
			User user = _userServices.GetUserIDByEmail(User.Identity.Name);
			projectsVM = _projectServices.GetProjectPostedByStudents(subject, userType, user.ID);
			ViewBag.UserID = user.ID;
			return View(projectsVM.OrderByDescending(a => a.ID));
		}

		[Authorize(Roles = "Supervisor, CoSupervisor")]
		[HttpGet, Route("projects/pickup")]
		public ActionResult Pickup(int projectID, int groupID)
		{
			//projectId->project, status->Registered, id for supervisor or coordinator
			//If is full, ApprovedNumber, send Email to all group member, Status->In process
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			User user = _userServices.GetUserIDByEmail(User.Identity.Name);

			var project = _projectServices.PickupStudentProject(userType, user.ID, projectID, groupID);
			if (project == false)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Pickup error", errorContent = "Can not found project !" });
			ViewBag.UserID = _userServices.GetUserIDByEmail(User.Identity.Name);
			return View();
		}

		[HttpGet, Route("projects/send-invitation-to-student")]
		public ActionResult SendInvitationToStudent(int projectID, int userID)
		{
			ViewBag.TypeOfUser = _userServices.GetCurrentRole(User.Identity.Name);
			_projectServices.SendInvitationToStudent(projectID, userID);
			return View();
		}

		[HttpGet, Route("projects/send-invitation-to-supervisor")]
		public ActionResult SendInvitationToSupervisor(int projectID, int userID, string userType)
		{
			ViewBag.TypeOfUser = _userServices.GetCurrentRole(User.Identity.Name);
			_projectServices.SendInvitationToSupervisor(projectID, userID);
			return View();
		}

		#endregion

		#region Keyword Matching Find 

		[HttpGet, Route("projects/find-students")]
		public ActionResult FindStudents(int id, string currentFilter, string searchString, int? page)
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;

			int PageSize = 20;
			if (searchString != null)
				page = 1;
			else
				searchString = currentFilter;

			ViewBag.CurrentFilter = searchString;
			int pageIndex = 1;
			pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

			var studentMatchedKeywordsList = _dbData.FindStudentsMatchProjectKeyword(id);
			List<MatchProjectKeywordViewModel> matchedKeywordsVMList = reorderMatchProjectKeywordToVM(studentMatchedKeywordsList);
			ViewBag.ProjectID = id;
			ViewBag.ProjectKeywords = getProjectKeywordsList(id);
			int pageNumber = (page ?? 1);
			return View(matchedKeywordsVMList.ToPagedList(pageNumber, PageSize));
		}

		[HttpGet, Route("projects/find-students-order-by-keywords")]
		public ActionResult FindStudentsOrderByKeywords(int id, string currentFilter, string searchString, int? page)
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;

			int PageSize = 20;
			if (searchString != null)
				page = 1;
			else
				searchString = currentFilter;

			ViewBag.CurrentFilter = searchString;
			int pageIndex = 1;
			pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

			var studentMatchKeywords = _dbData.FindStudentsMatchProjectKeywordOrderByKeywords(id);
			if (!String.IsNullOrEmpty(searchString))
			{
				studentMatchKeywords = studentMatchKeywords.Where(s => s.Title.ToLower().Contains(searchString.ToLower())
									|| s.Description.ToLower().Contains(searchString.ToLower())).ToList();

			}
			ViewBag.ProjectID = id;
			var projectKeywords = _commonServices.GetProjectSelectedKeywordsByProjectID(id);
			List<string> keywords = new List<string>();
			foreach (var item in projectKeywords)
			{
				keywords.Add(item.Title);
			}
			ViewBag.ProjectKeywords = keywords;
			int pageNumber = (page ?? 1);
			return View(studentMatchKeywords.ToPagedList(pageNumber, PageSize));
		}

		[Authorize(Roles = "Student, Supervisor, CoSupervisor, Coordinator")]
		[HttpGet, Route("projects/find-supervisors")]
		public ActionResult FindSupervisors(int id, string currentFilter, string searchString, int? page)
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;

			int PageSize = 20;
			if (searchString != null)
				page = 1;
			else
				searchString = currentFilter;

			ViewBag.CurrentFilter = searchString;
			int pageIndex = 1;
			pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

			var supervisorMatchedKeywordsList = _dbData.FindSupervisorsMatchProjectKeyword(id);
			List<MatchProjectKeywordViewModel> matchedKeywordsVMList = reorderMatchProjectKeywordToVM(supervisorMatchedKeywordsList);
			ViewBag.ProjectID = id;
			ViewBag.ProjectKeywords = getProjectKeywordsList(id);
			int pageNumber = (page ?? 1);
			return View(matchedKeywordsVMList.ToPagedList(pageNumber, PageSize));
		}

		#endregion

		#region Helper

		private List<string> getProjectKeywordsList(int projectID)
		{
			var projectKeywordsList = _commonServices.GetProjectSelectedKeywordsByProjectID(projectID).Select(a => a.Title).ToList();

			List<string> projectKeywords = new List<string>();
			foreach (var item in projectKeywordsList)
			{
				projectKeywords.Add(item);
			}
			return projectKeywords;
		}

		private List<MatchProjectKeywordViewModel> reorderMatchProjectKeywordToVM(List<MatchKeywordViewModel> matchedKeywordsList)
		{

			List<MatchProjectKeywordViewModel> matchedKeywordsVMList = new List<MatchProjectKeywordViewModel>();
			MatchProjectKeywordViewModel matchedKeywordsVM = new MatchProjectKeywordViewModel();
			List<Keyword> keywords = new List<Keyword>();
			Keyword keyword = null;
			var frontItem = matchedKeywordsList.FirstOrDefault();
			if (frontItem != null)
			{
				int frontUserID = frontItem.UserID;
				int counter = 0;

				foreach (var item in matchedKeywordsList)
				{
					int currentUserID = item.UserID;

					if (currentUserID != frontUserID)
					{
						matchedKeywordsVM.UserID = frontUserID;
						matchedKeywordsVM.MatchedKeywordCounter = counter;
						matchedKeywordsVM.Keywords = keywords;
						matchedKeywordsVM.UserType = item.UserType;
						matchedKeywordsVMList.Add(matchedKeywordsVM);
						counter = 0;
						keywords = new List<Keyword>();
						matchedKeywordsVM = new MatchProjectKeywordViewModel();
						frontUserID = currentUserID;
					}
					counter++;
					keyword = new Keyword(item.Title, item.Description);
					keyword.ID = item.KeywordID;
					keywords.Add(keyword);
				}
			}
			matchedKeywordsVMList = matchedKeywordsVMList.OrderBy(a => a.UserID).OrderByDescending(a => a.MatchedKeywordCounter).ToList();
			return matchedKeywordsVMList;
		}

		private ProjectPool getProjectPoolByProjectID(string userType, int projectID)
		{
			User user = _userServices.GetUserIDByEmail(User.Identity.Name);
			ProjectPool projectPool = new ProjectPool(user.ID);
			switch (userType)
			{
				case "Supervisor":
					Supervisor currentSupervisor = this.GetLoggedInUser() as Supervisor;
					projectPool = currentSupervisor.GetProjectPool(projectID);
					break;
				case "CoSupervisor":
					CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
					projectPool = currentCoSupervisor.GetProjectPool(projectID);
					break;
				case "ExternalSupervisor":
					ExternalSupervisor currentExtSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
					projectPool = currentExtSupervisor.GetProjectPool(projectID);
					break;
			}
			return projectPool;
		}

		private List<ProjectPoolEditModel> getMyProjectPoolVMList(string userType)
		{
			List<ProjectPoolEditModel> projectPoolsVM = new List<ProjectPoolEditModel>();
			List<ProjectPool> projectPools = new List<ProjectPool>();
			switch (userType)
			{
				case "Supervisor":
					Supervisor currentSupervisor = this.GetLoggedInUser() as Supervisor;
					projectPools = currentSupervisor.GetProjectPools().ToList();
					break;
				case "CoSupervisor":
					CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
					projectPools = currentCoSupervisor.GetProjectPools().ToList();
					break;
				case "ExternalSupervisor":
					ExternalSupervisor currentExtSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
					projectPools = currentExtSupervisor.GetProjectPools().ToList();
					break;
			}
			projectPools.ForEach(p =>
			{
				ProjectPoolEditModel model = _mapper.Map<ProjectPool, ProjectPoolEditModel>(p);
				projectPoolsVM.Add(model);
			});
			return projectPoolsVM.OrderByDescending(s => s.ID).ToList();
		}

		private List<ProjectViewModel> getMyProjectVMList(string userType)
		{
			List<ProjectViewModel> projectPoolsVM = new List<ProjectViewModel>();
			List<Project> projects = new List<Project>();
			switch (userType)
			{
				case "Supervisor":
					Supervisor currentSupervisor = this.GetLoggedInUser() as Supervisor;
					projects = currentSupervisor.GetProjects().OrderBy(a => a.SubjectID).ToList();
					break;
				case "CoSupervisor":
					CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
					projects = currentCoSupervisor.GetProjects().OrderBy(a => a.SubjectID).ToList();
					break;
				case "ExternalSupervisor":
					ExternalSupervisor currentExtSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
					projects = currentExtSupervisor.GetProjects().OrderBy(a => a.SubjectID).ToList();
					break;
			}
			projects.ForEach(p =>
			{
				ProjectViewModel model = _mapper.Map<Project, ProjectViewModel>(p);
				model.ProjectGroups = p.GetGroups();
				projectPoolsVM.Add(model);
			});
			return projectPoolsVM.OrderByDescending(s => s.ID).ToList();
		}

		private Subject getUserRegisteredCurrentSubject(string userType)
		{
			Subject subject = null;
			switch (userType)
			{
				case "CoSupervisor":
					CoSupervisor currentCoSupervisor = this.GetLoggedInUser() as CoSupervisor;
					subject = _commonServices.GetCurrentCoSupervisorEnrolledSubject(currentCoSupervisor);
					break;
				case "ExternalSupervisor":
					ExternalSupervisor currentExtSupervisor = this.GetLoggedInUser() as ExternalSupervisor;
					subject = _commonServices.GetCurrentExternalSupervisorEnrolledSubject(currentExtSupervisor);
					break;
			}
			return subject;
		}

		#endregion
	}
}
