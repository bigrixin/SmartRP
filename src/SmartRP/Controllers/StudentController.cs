using System.Web.Mvc;
using SmartRP.Domain;
using AutoMapper;
using SmartRP.Domain.Service;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System;
using PagedList;
using RazorEngine.Templating;

namespace SmartRP.Controllers
{
	[Authorize]
	public class StudentController : Controller
	{
		#region Fields

		private readonly ICommonService _commonServices;
		private readonly IProjectService _projectServices;
		private readonly IUserService _userServices;
		private readonly IMapper _mapper;
		private const int PageSize = 10;

		#endregion

		#region Ctor

		public StudentController(ICommonService commonServices, IProjectService projectServices, IUserService userServices, IMapper mapper)
		{
			_commonServices = commonServices;
			_projectServices = projectServices;
			_userServices = userServices;
			_mapper = mapper;
		}

		#endregion

		#region Menu

		[Authorize(Roles = "Student")]
		public ActionResult Menu()
		{
			Student currentStudent = this.GetLoggedInUser() as Student;
			DynamicViewBag model = _projectServices.GetRequestedCounterModel(currentStudent.ID, "Student");
			return PartialView("_StudentMenu", model);
		}

		#endregion

		#region Profile

		[HttpGet, Route("student/profile")]
		public new ActionResult Profile(int ID)
		{

			StudentViewModel model = new StudentViewModel();
			Student currentStudent = _userServices.GetCurrentStudent(ID);
			if (currentStudent == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Student profile error !", errorContent = "Can not find current student!" });

			model = _mapper.Map<Student, StudentViewModel>(currentStudent);
			model.UserSelectedKeywords = _commonServices.GetUserSelectedKeywordsByUserID(currentStudent.ID);
			model.EnrolledSubjects = _commonServices.GetEnrolledTermSubjectsVMList(currentStudent.ID, "Student");
			if (model.ResumeURL != null)
				model.ResumeURL = System.Text.RegularExpressions.Regex.Replace(model.ResumeURL, @"\s+", "%20");

			return View(model);
		}

		[Authorize(Roles = "Student")]
		public ActionResult MyAccount()
		{
			Student currentStudent = this.GetLoggedInUser() as Student;
			StudentViewModel model = new StudentViewModel();
			if (currentStudent != null)
			{
				model = _mapper.Map<Student, StudentViewModel>(currentStudent);
				model.UserSelectedKeywords = _commonServices.GetUserSelectedKeywordsByUserID(currentStudent.ID);
				model.EnrolledSubjects = _commonServices.GetEnrolledTermSubjectsVMList(currentStudent.ID, "Student");
			}
			return View(model);
		}

		[Authorize(Roles = "Student")]
		[HttpGet, Route("student/update-profile")]
		public ActionResult UpdateProfile(string userid)
		{
			Student currentStudent = this.GetLoggedInUser() as Student;
			StudentProfileEditModel model = new StudentProfileEditModel();
			if (currentStudent != null && currentStudent.LoginIdentityID == userid)
			{
				model = _mapper.Map<Student, StudentProfileEditModel>(currentStudent);
				model.KeywordList = _commonServices.GetKeywordList();
				model.SelectedKeywords = _commonServices.GetUserSelectedKeywordsByUserID(currentStudent.ID);
			}
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Resume", subjectAndUserID = currentStudent.StudentID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");
			return View(model);
		}

		[Authorize(Roles = "Student")]
		[HttpPost, Route("student/update-profile")]
		public ActionResult UpdateProfile(StudentProfileEditModel model)
		{
			Student currentStudent = this.GetLoggedInUser() as Student;
			if (currentStudent != null && ModelState.IsValid)
			{
				currentStudent = _mapper.Map(model, currentStudent);
				_commonServices.UpdateProfile(currentStudent, model.SelectedKeywordIDs);
			}
			return RedirectToAction("myaccount");
		}

		#endregion

		#region Subject

		[Authorize(Roles = "Student")]
		[HttpGet, Route("student/enrol-subject")]
		public ActionResult EnrolSubject()
		{
			ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;
			Subject currentEnrolledSubject = _commonServices.GetCurrentStudentEnrolledSubject(currentStudent);
			if (currentEnrolledSubject != null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			return RedirectToAction("Enrol", "Subjects", new { userID = currentStudent.LoginIdentityID });
		}

		#endregion

		#region Project

		[Authorize(Roles = "Student")]
		[HttpGet, Route("student/post-project")]
		public ActionResult PostProject()
		{
			string userType = "Student";
			ViewBag.TypeOfUser = userType;
			Student currentStudent = this.GetLoggedInUser() as Student;
			Subject currentEnrolledSubject = _commonServices.GetCurrentStudentEnrolledSubject(currentStudent);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			if (currentStudent.HasJoinedCurrentSubjectProjectGroup)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject post error !", errorContent = "You have joined a project group in this subject !" });

			//student can only post one project in one subject
			if (currentStudent.GetProjects().Where(a => a.SubjectID == currentEnrolledSubject.ID).Count() > 0)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject post error !", errorContent = "You have posted a project in this subject !" });

			Project project = new Project(currentStudent.ID);
			project.PublisherType = userType;
			ProjectEditModel model = _projectServices.MapperProjectToEditModel(project, currentStudent.ID, userType);
			model.SubjectName = currentEnrolledSubject.SubjectName;
			model.ExpiredAt = _commonServices.GetCurrentOpenTerm().EndAt;
			model.CurrentEnrolledSubjectID = currentEnrolledSubject.ID;
            model.MaxGroupNumber = 1;
            
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Project", subjectAndUserID = currentStudent.StudentID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");

			return View(model);
		}

		[HttpPost, Route("student/post-project")]
		public ActionResult PostProject(ProjectEditModel model)
		{
			Student currentStudent = this.GetLoggedInUser() as Student;
			if (currentStudent != null && ModelState.IsValid)
				_projectServices.AddPostedProjectByStudent(currentStudent, model);
			return RedirectToAction("my-posted-projects");
		}

		[Authorize(Roles = "Student")]
		[HttpGet, Route("student/update-project")]
		public ActionResult UpdateProject(int projectID)
		{
			ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;
			Subject currentEnrolledSubject = _commonServices.GetCurrentStudentEnrolledSubject(currentStudent);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			Project project = currentStudent.GetProject(projectID);
			if (project == null)
				return RedirectToAction("my-posted-projects");

			ProjectEditModel model = _projectServices.MapperProjectToEditModel(project, currentStudent.ID, "Student");
			model.SubjectName = currentEnrolledSubject.SubjectName;
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Project", subjectAndUserID = currentStudent.StudentID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");

			return View(model);
		}


		[HttpPost, Route("student/update-project")]
		public ActionResult UpdateProject(ProjectEditModel model)
		{
			Student currentStudent = this.GetLoggedInUser() as Student;
			if (currentStudent != null && ModelState.IsValid)
				_projectServices.UpdatePostedProjectByStudent(currentStudent, model);
			return RedirectToAction("my-posted-projects");
		}

		[HttpGet]
		[Authorize(Roles = "Student")]
		public ActionResult DeleteProject(int? projectID, int groupID)
		{
			ViewBag.TypeOfUser = "Student";
			if (projectID == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			Student currentStudent = this.GetLoggedInUser() as Student;
			Subject currentEnrolledSubject = _commonServices.GetCurrentStudentEnrolledSubject(currentStudent);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			Project project = currentStudent.GetProject((int)projectID);
			if (project == null)
				return RedirectToAction("my-posted-projects");

			int groupMembers = project.GetGroups().LastOrDefault().GetStudents().Count();
			if (groupMembers > 1)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Delete project error!", errorContent = "Can not delete the project, because someone have joined this group !" });

			var model = _mapper.Map<Project, ProjectEditModel>(project);
			model.KeywordList = _commonServices.GetKeywordList();
			model.PublisherType = "Student";
			model.UserID = currentStudent.ID;

			return View(model);
		}

		// POST: 
		[HttpPost, ActionName("DeleteProject")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int projectID, int groupID)
		{
			ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;
			if (currentStudent != null && ModelState.IsValid)
				_projectServices.DeletePostedProjectGroupByStudent(currentStudent, projectID, groupID);

			return RedirectToAction("my-posted-projects");
		}

		[Authorize(Roles = "Student")]
		[HttpGet, Route("student/my-posted-projects")]
		public ActionResult MyPostedProject()
		{
			ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;
			Subject currentEnrolledSubject = _commonServices.GetCurrentStudentEnrolledSubject(currentStudent);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			List<ProjectViewModel> projectsVM = getCurrentStudentPostedProjectListVM(currentStudent);
			return View(projectsVM);
		}

		[Authorize(Roles = "Student")]
		[HttpGet, Route("student/project-list")]
		public ActionResult ProjectList()
		{
			//		ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;
			Subject currentEnrolledSubject = _commonServices.GetCurrentStudentEnrolledSubject(currentStudent);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			return RedirectToAction("project-list", "projects", new { subjectID = currentEnrolledSubject.ID });
		}

		#endregion

		#region Request

		[Authorize(Roles = "Student")]
		[HttpGet, Route("student/request-to-join-group")]
		public ActionResult RequestToJoinGroup(int projectID, int groupID)
		{
			ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;
			Subject currentEnrolledSubject = _commonServices.GetCurrentStudentEnrolledSubject(currentStudent);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			var lastEnrolledGroup = currentStudent.GetEnrolledGroups().LastOrDefault();
			if (lastEnrolledGroup != null)
			{

				// check if student has enrolled a group in current subject
				if (_projectServices.IsCurrentSubjectProject(currentEnrolledSubject.ID, lastEnrolledGroup.ProjectID))
					return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Enrol group error !", errorContent = "You have enrolled a group in this subject !" });
			}
			else
			{
			  // check if has sent a request to join the project
				if (_projectServices.HasSentRequest(currentStudent.ID, projectID,groupID))
					return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Send request error !", errorContent = "You have sent a request to the project!" });

			}
			ProjectViewModel model = _projectServices.MapperProjectGroupToViewModel(projectID, groupID);

			return View(model);
		}

		[HttpPost, Route("student/request-to-join-group")]
		public ActionResult ComfirmRequestToJoinGroup(ProjectViewModel model)
		{
			ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;
			Subject currentEnrolledSubject = _commonServices.GetCurrentStudentEnrolledSubject(currentStudent);
			if (currentEnrolledSubject == null || !ModelState.IsValid)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			if (!_projectServices.StudentRequestToJoinAProjectGroup(currentStudent, currentEnrolledSubject, model))
				return RedirectToAction("ErrorAlert", "Error", new
				{
					errorTitle = "Request to Join Group Error",
					errorContent = "You have already send a request! Go to My Requested List to check details."
				});

			return RedirectToAction("My-Requested-List");
		}

		[Authorize(Roles = "Student")]
		[HttpGet, Route("student/my-requested-list")]
		public ActionResult MyRequestedList()
		{
			ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;
			Subject currentEnrolledSubject = _commonServices.GetCurrentStudentEnrolledSubject(currentStudent);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			var requestedListVM = _projectServices.GetStudentRequestedList(currentEnrolledSubject.ID, currentStudent);
			return View(requestedListVM);
		}

		[HttpGet, Route("student/manage-request")]
		public ActionResult ManageRequest()
		{
			ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;
			Subject currentEnrolledSubject = _commonServices.GetCurrentStudentEnrolledSubject(currentStudent);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });
			var requestedListVM = _projectServices.GetMyPostedProjectJoinGroupRequestVMList(currentEnrolledSubject.ID, currentStudent.ID);
			return View(requestedListVM);
		}

		#endregion

		#region Group

		[Authorize(Roles = "Student")]
		[HttpGet, Route("student/my-project-group")]
		public ActionResult MyProjectGroup()
		{
			ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;
			Subject currentEnrolledSubject = _commonServices.GetCurrentStudentEnrolledSubject(currentStudent);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			List<GroupViewModel> groupsVM = _projectServices.GetCurrentSubjectStudentJoinedGroupsVMList(currentEnrolledSubject, currentStudent);
			if (groupsVM == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Group error !", errorContent = "Can not find current subject joined group !" });
			ViewBag.UserID = currentStudent.ID;
			return View(groupsVM);
		}

		[HttpGet, Route("student/confirm-join-group")]
		public ActionResult ConfirmJoinGroup(int currentUserID, int studentID, int projectID, int groupID, string actionWord)
		{
			ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;
			Subject currentEnrolledSubject = _commonServices.GetCurrentStudentEnrolledSubject(currentStudent);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			//check if the student has join a group in the subject
			var groupsVM = _projectServices.GetCurrentSubjectStudentJoinedGroupsVMList(currentEnrolledSubject, currentStudent);
			if (groupsVM.Count() != 0)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Process request error", errorContent = "You have joined a group !" });
			else
				return RedirectToAction("process-request", "projects", new { currentUserID = currentUserID, studentID = studentID, projectID = projectID, groupID = groupID, actionWord = "Register" });
		}

		[Authorize(Roles = "Student")]
		[HttpGet, Route("student/withdraw-group")]
		public ActionResult WithdrawGroup(int groupID)
		{
			ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;
			Subject currentEnrolledSubject = _commonServices.GetCurrentStudentEnrolledSubject(currentStudent);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			Group group = currentStudent.GetGroup(groupID);
			GroupViewModel groupVM = _mapper.Map<Group, GroupViewModel>(group);
			groupVM.JoinedStudents = group.GetStudents();
			if (groupVM == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Group error !", errorContent = "Can not find current group !" });

			return View(groupVM);
			//return RedirectToAction("my-project-group");
		}

		[HttpGet, Route("student/confirm-to-withdraw-group")]
		public ActionResult ConfirmToWithdrawGroup(int projectID, int groupID)
		{
			ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;
			Subject currentEnrolledSubject = _commonServices.GetCurrentStudentEnrolledSubject(currentStudent);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			_projectServices.StudentWithdrawGroup(currentStudent, projectID, groupID);
			ViewBag.UserID = currentStudent.ID;
			return RedirectToAction("my-project-group");
		}

		[Authorize(Roles = "Student")]
		[HttpGet, Route("student/edit-group")]
		public ActionResult EditGroup(int projectID, int groupID)
		{
			ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;
			Subject currentEnrolledSubject = _commonServices.GetCurrentStudentEnrolledSubject(currentStudent);
			if (currentEnrolledSubject == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Subject enrol error !", errorContent = "Can not find current student enrolled subject !" });

			Group group = currentStudent.GetGroup(groupID);
			GroupViewModel groupVM = _mapper.Map<Group, GroupViewModel>(group);

			return View(groupVM);
		}

		[HttpPost, Route("student/edit-group")]
		public ActionResult EditGroup(GroupViewModel model)
		{
			Student currentStudent = this.GetLoggedInUser() as Student;
			if (currentStudent != null && ModelState.IsValid)
				_projectServices.UpdateGroupInforByStudent(currentStudent, model);
			return RedirectToAction("my-project-group");
		}

		#endregion

		#region Group Report

		[Authorize(Roles = "Student")]
		[HttpGet, Route("student/upload-group-report")]
		public ActionResult UploadGroupReport(int groupID)
		{
			ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;
			ReportViewModel model = new ReportViewModel();
			model.GroupID = groupID;
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Report", subjectAndUserID = currentStudent.StudentID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");
			return View(model);
		}

		[ValidateAntiForgeryToken]
		[HttpPost, Route("student/upload-group-report")]
		public ActionResult UploadGroupReport(ReportViewModel model)
		{
			Student currentStudent = this.GetLoggedInUser() as Student;
			if (currentStudent == null || !ModelState.IsValid)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Upload Error", errorContent = "Can not uplaod report!" });

			_projectServices.UploadReportByStudent(currentStudent, model);
			return RedirectToAction("my-project-group");
		}

		[Authorize(Roles = "Student")]
		[HttpGet, Route("student/update-group-report")]
		public ActionResult UpdateGroupReport(int groupID, int reportID)
		{
			ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;

			Report report = currentStudent.GetGroup(groupID).GetReport(reportID);
			var model = _mapper.Map<Report, ReportViewModel>(report);
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Report", subjectAndUserID = currentStudent.StudentID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");
			return View(model);
		}

		[ValidateAntiForgeryToken]
		[HttpPost, Route("student/update-group-report")]
		public ActionResult UpdateGroupReport(ReportViewModel model)
		{
			Student currentStudent = this.GetLoggedInUser() as Student;
			if (currentStudent == null || !ModelState.IsValid)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Upload Error", errorContent = "Can not uplaod report!" });

			_projectServices.UpdateReportByStudent(currentStudent, model);
			return RedirectToAction("my-project-group");
		}

		[Authorize(Roles = "Student")]
		[HttpGet, Route("student/delete-group-report")]
		public ActionResult DeleteGroupReport(int groupID, int reportID)
		{
			ViewBag.TypeOfUser = "Student";
			Student currentStudent = this.GetLoggedInUser() as Student;

			Report report = currentStudent.GetGroup(groupID).GetReport(reportID);
			var model = _mapper.Map<Report, ReportViewModel>(report);
			ViewBag.PathUpload = Url.Action("UploadFileToServer", "Upload", new { fileString = "Report", subjectAndUserID = currentStudent.StudentID });
            ViewBag.PathDelete = Url.Action("DeleteFileFromServer", "Upload");
			return View(model);
		}

		[ValidateAntiForgeryToken]
		[HttpPost, Route("student/delete-group-report")]
		public ActionResult DeleteGroupReportConfirmed(int groupID, int reportID)
		{
			Student currentStudent = this.GetLoggedInUser() as Student;
			if (currentStudent == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Delete Error", errorContent = "Can not current student!" });

			_projectServices.DeleteReportByStudent(currentStudent, groupID, reportID);
			return RedirectToAction("my-project-group");
		}

		#endregion

		#region Status

		[Authorize(Roles = "Coordinator")]
		[HttpGet, Route("student/current-status")]
		public ActionResult CurrentStatus(string currentFilter, string searchString, int? page)
		{
			if (searchString != null)
				page = 1;
			else
				searchString = currentFilter;

			ViewBag.CurrentFilter = searchString;
			int pageIndex = 1;
			pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

			CurrentTermStudentViewModel model = new CurrentTermStudentViewModel();
			var studentsVM = new List<CurrentTermStudentViewModel>();
			Term currentTerm = _commonServices.GetCurrentOpenTerm();
			if (currentTerm == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Term error", errorContent = "Can not found current term !" });
			var currentTermStudents = _commonServices.GetCurrentSemesterStudents().ToList();
			ViewBag.termID = currentTerm.ID;

			///this part need to change

			if (!String.IsNullOrEmpty(searchString))
			{
				currentTermStudents = currentTermStudents.Where(s => s.Lastname.ToLower().Contains(searchString.ToLower()) ||
								s.StudentID.ToLower().Contains(searchString.ToLower()) ||
								s.Email.ToLower().Contains(searchString.ToLower())).ToList();
			}

			currentTermStudents.ForEach(j =>
			{
				if (!j.HasJoinedCurrentSubjectProjectGroup)
				{
					var vm = _mapper.Map<Student, CurrentTermStudentViewModel>(j);
					studentsVM.Add(vm);
				}
			});

			studentsVM.Sort((x, y) => string.Compare(x.Status.ToString(), y.Status.ToString()));
			ViewBag.TypeOfUser = this._userServices.GetCurrentRole(User.Identity.Name);

			int pageNumber = (page ?? 1);
			return View(studentsVM.ToPagedList(pageNumber, PageSize));
		}


		#endregion

		#region Helper

		private List<ProjectViewModel> getCurrentStudentPostedProjectListVM(Student student)
		{
			var projectsVM = new List<ProjectViewModel>();
			var projects = student.GetProjects();
			projects.ForEach(p =>
			{
				ProjectViewModel model = _mapper.Map<Project, ProjectViewModel>(p);

				///	model.JoinedStudents = p.GetGroups().SingleOrDefault().GetStudents().Count();
				model.ProjectGroups = p.GetGroups();
				projectsVM.Add(model);
			});
			return projectsVM;
		}

 

        #endregion
    }
}