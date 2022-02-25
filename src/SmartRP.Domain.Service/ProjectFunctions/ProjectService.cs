using AutoMapper;
using RazorEngine.Templating;
using SmartRP.Infrastructure;
using SmartRP.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SmartRP.Domain.Service
{
	public class ProjectService : IProjectService
	{
		#region Fields

		private readonly IWriteEntities _writeEntities;
		private readonly IReadEntities _readEntities;
		private readonly ICommonService _commonServices;
		private readonly IUserService _userServices;
		private readonly IUploadService _uploadServices;
		private readonly DbData _dbData;
		private readonly IMapper _mapper;

		#endregion

		#region Ctor

		public ProjectService(IWriteEntities writeEntities, IReadEntities readEntities, IUserService userServices, ICommonService commonServices, IUploadService uploadService, DbData dbData, IMapper mapper)
		{
			_writeEntities = writeEntities;
			_readEntities = readEntities;
			_commonServices = commonServices;
			_userServices = userServices;
			_uploadServices = uploadService;
			_dbData = dbData;
			_mapper = mapper;
		}

		#endregion

		#region Project Pool - supervisor, co-supervisor, ext-supervisor

		public bool AddProjectToPool(string userType, int userID, ProjectPoolEditModel model)
		{
			ProjectPool projectPool = new ProjectPool(userID);
			projectPool = _mapper.Map(model, projectPool);

			switch (userType)
			{
				case "Supervisor":
					Supervisor currentSupervisor = _readEntities.Get<Supervisor>(s => s.ID == userID).SingleOrDefault();
					if (currentSupervisor == null)
						return false;
					currentSupervisor.AddProjectPool(projectPool);
					_writeEntities.Update(currentSupervisor);
					break;
				case "CoSupervisor":
					Supervisor currentCoSupervisor = _readEntities.Get<CoSupervisor>(s => s.ID == userID).SingleOrDefault();
					if (currentCoSupervisor == null)
						return false;
					currentCoSupervisor.AddProjectPool(projectPool);
					_writeEntities.Update(currentCoSupervisor);

					break;
				case "ExternalSupervisor":
					Supervisor currentExternalSupervisor = _readEntities.Get<ExternalSupervisor>(s => s.ID == userID).SingleOrDefault();
					if (currentExternalSupervisor == null)
						return false;
					currentExternalSupervisor.AddProjectPool(projectPool);
					_writeEntities.Update(currentExternalSupervisor);
					break;
			}

			_writeEntities.Save();
			return true;
		}

		public bool UpdateProjectOfPool(string userType, int userID, ProjectPoolEditModel model)
		{
			ProjectPool projectPool = _readEntities.Get<ProjectPool>(p => p.ID == model.ID).SingleOrDefault();
			projectPool = _mapper.Map(model, projectPool);
			projectPool.UpdatedAt = DateTime.Now;
			switch (userType)
			{
				case "Supervisor":
					Supervisor currentSupervisor = _readEntities.Get<Supervisor>(s => s.ID == userID).SingleOrDefault();
					if (currentSupervisor == null)
						return false;
					currentSupervisor.UpdateProjectPool(projectPool);
					_writeEntities.Update(currentSupervisor);
					break;
				case "CoSupervisor":
					Supervisor currentCoSupervisor = _readEntities.Get<CoSupervisor>(s => s.ID == userID).SingleOrDefault();
					if (currentCoSupervisor == null)
						return false;
					currentCoSupervisor.UpdateProjectPool(projectPool);
					_writeEntities.Update(currentCoSupervisor);

					break;
				case "ExternalSupervisor":
					Supervisor currentExternalSupervisor = _readEntities.Get<ExternalSupervisor>(s => s.ID == userID).SingleOrDefault();
					if (currentExternalSupervisor == null)
						return false;
					currentExternalSupervisor.UpdateProjectPool(projectPool);
					_writeEntities.Update(currentExternalSupervisor);
					break;
			}

			_writeEntities.Save();
			return true;
		}

		public void DeletePostedProjectPool(string userType, int userID, int projectPoolID)
		{
			ProjectPool projectPool = new ProjectPool(userID);
			switch (userType.ToLower())
			{
				case "supervisor":
					Supervisor currentSupervisor = _readEntities.Get<Supervisor>(s => s.ID == userID).SingleOrDefault();
					projectPool = currentSupervisor.GetProjectPool(projectPoolID);
					break;
				case "cosupervisor":
					Supervisor currentCoSupervisor = _readEntities.Get<CoSupervisor>(s => s.ID == userID).SingleOrDefault();
					projectPool = currentCoSupervisor.GetProjectPool(projectPoolID);
					break;
				case "externalsupervisor":
					Supervisor currentExternalSupervisor = _readEntities.Get<ExternalSupervisor>(s => s.ID == userID).SingleOrDefault();
					projectPool = currentExternalSupervisor.GetProjectPool(projectPoolID);
					break;
			}
			if (projectPool != null)
			{
				// DocumentURL is used by ProjectPool and Project table, need improve here
				//_uploadServices.DeleteFromServer(projectPool.DocumentURL);  
				_writeEntities.Delete(projectPool);
				_writeEntities.Save();
			}
		}

		#endregion

		#region Mapper

		public ProjectEditModel MapperProjectToEditModel(Project project, int userID, string userType)
		{
			int studentsCount = 0;
			List<Subject> subjects = _commonServices.GetCurrentOpenTerm().GetSubjects();
			//here has change
			if (userType != "Supervisor")
				subjects = _commonServices.GetEnrolledTermSubjects(userID, userType);

			ProjectEditModel model = _mapper.Map<Project, ProjectEditModel>(project);
			model.KeywordList = _commonServices.GetKeywordList();
			model.SelectedKeywords = _commonServices.GetProjectSelectedKeywordsByProjectID(project.ID);
			var subject = subjects.Where(a => a.ID == project.SubjectID).SingleOrDefault();
			if (subject != null)
				model.SubjectList = subjects.Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.SubjectName.ToString(), Selected = (x.SubjectName == subject.SubjectName) ? true : false }).ToList();
			else
				model.SubjectList = subjects.Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.SubjectName.ToString() }).ToList();

			if (model.SubjectList.Count() == 1)
			{
				model.SubjectID = Convert.ToInt32(model.SubjectList.FirstOrDefault().Value);
				model.SubjectList = subjects.Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.SubjectName.ToString(), Selected = true }).ToList();

				var subjectNameValues = Enum.GetValues(typeof(SubjectName));
				foreach (SubjectName item in subjectNameValues)
				{
					if (Convert.ToInt32(item).Equals(model.SubjectID))
					{
						model.SubjectName = item;
						break;
					}
				}
			}

			foreach (var group in project.GetGroups())
			{
				if (studentsCount < group.GetStudents().Count())
					studentsCount = group.GetStudents().Count();
			}

			model.JoinedGroupMaxStudents = studentsCount;

			return model;
		}

		public ProjectViewModel MapperProjectToViewModel(int projectID)
		{
			Project project = _readEntities.Get<Project>(p => p.ID == projectID).SingleOrDefault();
			if (project == null)
				return null;
			ProjectViewModel model = _mapper.Map<Project, ProjectViewModel>(project);
			model.SelectedKeywords = _commonServices.GetProjectSelectedKeywordsByProjectID(project.ID);
			model.ProjectGroups = project.GetGroups();
			if (model.DocumentURL != null)
				model.DocumentURL = System.Text.RegularExpressions.Regex.Replace(model.DocumentURL, @"\s+", "%20");
			return model;
		}

		public ProjectPoolEditModel MapperProjectPoolToViewModel(string userType, int userID, int projectID)
		{
			ProjectPool project = new ProjectPool(userID);
			switch (userType.ToLower())
			{
				case "supervisor":
					Supervisor currentSupervisor = _readEntities.Get<Supervisor>(s => s.ID == userID).SingleOrDefault();
					project = currentSupervisor.GetProjectPool(projectID);
					break;
				case "cosupervisor":
					Supervisor currentCoSupervisor = _readEntities.Get<CoSupervisor>(s => s.ID == userID).SingleOrDefault();
					project = currentCoSupervisor.GetProjectPool(projectID);
					break;
				case "externalsupervisor":
					Supervisor currentExternalSupervisor = _readEntities.Get<ExternalSupervisor>(s => s.ID == userID).SingleOrDefault();
					project = currentExternalSupervisor.GetProjectPool(projectID);
					break;
			}

			ProjectPoolEditModel model = _mapper.Map<ProjectPool, ProjectPoolEditModel>(project);
			////	model.SelectedKeywords = _commonServices.GetProjectSelectedKeywordsByProjectID(project.ID);
			if (model.DocumentURL != null)
				model.DocumentURL = System.Text.RegularExpressions.Regex.Replace(model.DocumentURL, @"\s+", "%20");
			return model;
		}

		public ProjectViewModel MapperProjectGroupToViewModel(int projectID, int groupID)
		{
			ProjectViewModel model = MapperProjectToViewModel(projectID);
			Project project = _readEntities.Get<Project>(p => p.ID == projectID).SingleOrDefault();
			if (project == null)
				return null;
			if (groupID != 0)
			{
				model.GroupName = project.GetGroup(groupID).Name;
				model.GroupDescription = project.GetGroup(groupID).Description;
				model.GroupSize = project.GetGroup(groupID).Size;
				model.GroupMembersList = project.GetGroup(groupID).GetStudents();
				model.ProjectGroups = project.GetGroups();
				model.GroupID = groupID;
			}
			return model;
		}

		public GradeGroupViewModel MapperGradeGroupToViewModel(int groupID)
		{
			Group group = _readEntities.Get<Group>(p => p.ID == groupID).SingleOrDefault();
			if (group == null)
				return null;
			GradeGroupViewModel model = _mapper.Map<Group, GradeGroupViewModel>(group);
			return model;
		}

		#endregion

		#region Supervisor

		public bool AddPostedProjectBySupervisor(Supervisor currentSupervisor, ProjectEditModel model)
		{
			Project project = new Project(currentSupervisor.ID, "Supervisor");
			project = _mapper.Map(model, project);
			project = addSelectKeywordToProject(project, model.SelectedKeywordIDs);
			Term currentTerm = _commonServices.GetCurrentOpenTerm();
			int subjectID = currentTerm.GetSubjectIDByName(model.SubjectName);
			if (subjectID == 0)
				return false;
			project.SubjectID = subjectID;

			Subject currentSubject = currentTerm.GetSubject(subjectID);
			currentSubject.AddProject(project);

			//add group to project
			Group group = new Group(project.ID, "Group 1", "First group description", model.GroupSize);
			group.SetSupervisor(currentSupervisor.ID);
			project.AddGroup(group);

			currentSupervisor.AddProject(project);
			_writeEntities.Update(currentSupervisor);
			_writeEntities.Save();
			return true;
		}

		public bool UpdatePostedProjectBySupervisor(int userID, ProjectEditModel model)
		{
			User currentUser = _readEntities.Get<User>(u => u.ID == userID).SingleOrDefault();
			if (currentUser == null)
				return false;
			Project project = currentUser.GetProject(model.ID);
			int currentMaxGroupSize = project.GroupSize;
			if (project == null)
				return false;
			project = _mapper.Map(model, project);
			//update selected subject
			Term currentTerm = _commonServices.GetCurrentOpenTerm();
			int subjectID = currentTerm.GetSubjectIDByName(model.SubjectName);
			if (subjectID == 0)
				return false;
			project.SubjectID = subjectID;

			//update selected keyword to project 
			project = addSelectKeywordToProject(project, model.SelectedKeywordIDs);

			//update group size, status and project status
			project = updateProjectStatus(project, model.GroupSize);

			currentUser.UpdateProject(project);
			_writeEntities.Update(currentUser);
			_writeEntities.Save();

			delectUnselectItems(model.SelectedKeywordIDs, project);
			return true;
		}

		#endregion

		#region All Supervisor

		public bool AddPostedProject(string userType, int userID, ProjectEditModel model)
		{
			Project project = new Project(userID, userType);
			project = _mapper.Map(model, project);
			project = addSelectKeywordToProject(project, model.SelectedKeywordIDs);

			if (model.SubjectID == 0)
				return false;
			Term currentTerm = _commonServices.GetCurrentOpenTerm();
			Subject currentSubject = currentTerm.GetSubject(model.SubjectID);
			currentSubject.AddProject(project);

			//add group to project
			Group group = new Group(project.ID, "Group 1", "First group description", model.GroupSize);

			switch (userType)
			{
				case "Supervisor":
					Supervisor currentSupervisor = _readEntities.Get<Supervisor>(a => a.ID == userID).SingleOrDefault();
					if (currentSupervisor == null)
						return false;
					group.SetSupervisor(currentSupervisor.ID);
					project.AddGroup(group);
					currentSupervisor.AddProject(project);
					_writeEntities.Update(currentSupervisor);
					_writeEntities.Save();
					break;
				case "CoSupervisor":
					CoSupervisor currentCoSupervisor = _readEntities.Get<CoSupervisor>(a => a.ID == userID).SingleOrDefault();
					if (currentCoSupervisor == null)
						return false;
					group.SetCoSupervisor(currentCoSupervisor.ID);
					group.SetSupervisor(currentCoSupervisor.MySupervisorID);
					project.AddGroup(group);
					//add skills, add references here, may need later
					currentCoSupervisor.AddProject(project);
					_writeEntities.Update(currentCoSupervisor);
					_writeEntities.Save();
					break;
				case "ExternalSupervisor":
					ExternalSupervisor currentExtSupervisor = _readEntities.Get<ExternalSupervisor>(a => a.ID == userID).SingleOrDefault();
					if (currentExtSupervisor == null)
						return false;
					group.SetExternalSupervisor(currentExtSupervisor.ID);
					group.SetSupervisor(currentExtSupervisor.MySupervisorID);
					project.AddGroup(group);
					currentExtSupervisor.AddProject(project);
					_writeEntities.Update(currentExtSupervisor);
					_writeEntities.Save();
					break;
			}

			return true;
		}

		public void UpdatePostedProject(int userID, ProjectEditModel model)
		{
			//for Co-supervisor & Ext-supervisor
			User currentUser = _readEntities.Get<User>(u => u.ID == userID).SingleOrDefault();
			if (currentUser != null)
			{
				Project project = currentUser.GetProject(model.ID);
				if (project != null)
				{
					project = _mapper.Map(model, project);

					//update selected keyword to project 
					project = addSelectKeywordToProject(project, model.SelectedKeywordIDs);

					//update group size, status and project status
					project = updateProjectStatus(project, model.GroupSize);

					//update group to project
					currentUser.UpdateProject(project);
					_writeEntities.Update(currentUser);
					_writeEntities.Save();

					delectUnselectItems(model.SelectedKeywordIDs, project);
				}
			}
		}

		public void DeletePostedProjectGroup(int userID, int projectID, int groupID)
		{
			User currentUser = _readEntities.Get<User>(u => u.ID == userID).SingleOrDefault();
			if (currentUser != null)
			{
				Project project = currentUser.GetProject(projectID);
				if (project != null)
				{
					// DocumentURL is used by ProjectPool and Project table, need improve here
					//_uploadServices.DeleteFromServer(project.DocumentURL);
					if (project.GetGroups().Count() == 1)
					{
						_writeEntities.Delete(project);
					}
					else
					{
						//need test
						Group group = project.GetGroup(groupID);
						_writeEntities.Delete(group);
					}

					List<JoinProjectGroup> joinProjectGroups = _readEntities.Get<JoinProjectGroup>(a => a.ProjectID == project.ID).ToList();
					foreach (var joinProjectGroup in joinProjectGroups)
					{
						joinProjectGroup.RequestStatus = RequestStatus.Deleted;
						_writeEntities.Update(joinProjectGroup);
					}
					_writeEntities.Update(currentUser);
					_writeEntities.Save();
				}
			}
		}

		#endregion

		#region Student

		public void AddPostedProjectByStudent(Student currentStudent, ProjectEditModel model)
		{
			Project project = new Project(currentStudent.ID);
			project = _mapper.Map(model, project);
			project = addSelectKeywordToProject(project, model.SelectedKeywordIDs);
			project.Status = ProjectStatus.Pending;
			project.ProjectPoolID = 0;

			//add group to project
			Group group = new Group(project.ID, currentStudent.ID, model.GroupName, model.GroupDescription, model.GroupSize);
			group.AddStudent(currentStudent);   //group add a student
			currentStudent.EnrolGroup(group);   //student join a group
			project.AddGroup(group);

			Subject currentSubject = currentStudent.GetSubjectByID(model.CurrentEnrolledSubjectID);
			currentSubject.AddProject(project);
			//add skills and references
			currentStudent.AddProject(project);
			_writeEntities.Update(currentStudent);
			_writeEntities.Save();
		}

		public void UpdatePostedProjectByStudent(Student currentStudent, ProjectEditModel model)
		{
			Project project = currentStudent.GetProject(model.ID);
			int currentMaxGroupSize = project.GroupSize;
			if (project != null)
			{
				project = _mapper.Map(model, project);

				//update selected keyword to project 
				project = addSelectKeywordToProject(project, model.SelectedKeywordIDs);

				//update group size (student can only post one project and one group in a subject)
				Group group = project.GetGroups().LastOrDefault();
				if (group != null)
					project = updateProjectGroupStatus(project, group, model.GroupSize);
				currentStudent.UpdateProject(project);
				_writeEntities.Update(currentStudent);
				_writeEntities.Save();

				delectUnselectItems(model.SelectedKeywordIDs, project);
			}
		}

		public void DeletePostedProjectGroupByStudent(Student currentStudent, int projectID, int groupID)
		{
			Project project = currentStudent.GetProject(projectID);
			if (project != null)
			{
				if (!String.IsNullOrEmpty(project.DocumentURL))
					_uploadServices.DeleteFromServer(project.DocumentURL);
				if (project.GetGroups().Count() == 1)
				{
					_writeEntities.Delete(project);
				}
				else
				{
					//need test
					Group group = project.GetGroup(groupID);
					_writeEntities.Delete(group);
				}
				currentStudent.HasJoinedCurrentSubjectProjectGroup = false;
				List<JoinProjectGroup> joinProjectGroups = _readEntities.Get<JoinProjectGroup>(a => a.ProjectID == project.ID).ToList();
				foreach (var joinProjectGroup in joinProjectGroups)
				{
					joinProjectGroup.RequestStatus = RequestStatus.Deleted;
					_writeEntities.Update(joinProjectGroup);
				}
				_writeEntities.Update(currentStudent);
				_writeEntities.Save();
			}
		}

		#endregion

		#region Project
		public IEnumerable<Project> GetCurrentSubjectProjectList(int subjectID)
		{
			Term currentTerm = _commonServices.GetCurrentOpenTerm();
			if (currentTerm == null)
				return null;
			Subject currentSubject = currentTerm.GetSubject(subjectID);
			if (currentSubject == null)
				return null;
			var projects = currentSubject.GetProjects();
			return projects;
		}

		public IEnumerable<Project> GetCurrentSemesterProjectList()
		{
			Term currentTerm = _commonServices.GetCurrentOpenTerm();
			if (currentTerm == null)
				return null;
			IEnumerable<Project> projects = null;
			int i = 0;
			foreach (var subject in currentTerm.GetSubjects())
			{
				if (i == 0)
					projects = subject.GetProjects();
				else
				{
					var nextProjects = subject.GetProjects();
					if (nextProjects != null)
						projects = projects.Concat(nextProjects);
				}

				i++;
			}
			return projects;
		}

		public IEnumerable<Project> GetCurrentEnrolledSemesterByCoSupervisorProjectList(int coSupervisorID)
		{
			Term currentTerm = _commonServices.GetCurrentOpenTerm();
			if (currentTerm == null)
				return null;
			CoSupervisor coSupervisor = _readEntities.Get<CoSupervisor>(a => a.ID == coSupervisorID).SingleOrDefault();

			var subjects = coSupervisor.GetCurrentTermEnrolledSubjects(currentTerm.ID);
			IEnumerable<Project> projects = null;
			int i = 0;
			foreach (var subject in subjects)
			{
				if (i == 0)
					projects = subject.GetProjects();
				else
				{
					var nextProjects = subject.GetProjects();
					if (nextProjects != null)
						projects = projects.Concat(nextProjects);
				}

				i++;
			}
			return projects;
		}

		public List<ProjectViewModel> GetProjectPostedByStudents(Subject subject, string userType, int superUserID)
		{
			List<ProjectViewModel> projectVM = new List<ProjectViewModel>();
			IEnumerable<Project> projects = null;
			if (userType == "Supervisor")    //for supervisor
				projects = GetCurrentSemesterProjectList();
			else if (userType == "CoSupervisor")
			{
				projects = GetCurrentEnrolledSemesterByCoSupervisorProjectList(superUserID);
			}
			else
				projects = GetCurrentSubjectProjectList(subject.ID);

			if (projects.Count() > 0)
				projectVM = mapProjectPostedByStudentToVMList(projects);
			return projectVM;
		}

		public bool IsCurrentSubjectProject(int subjectID, int projectID)
		{
			Project project = _readEntities.Get<Project>(p => p.ID == projectID).SingleOrDefault();
			if (project == null)
				return false;
			if (project.SubjectID == subjectID)
				return true;
			return false;
		}

		public bool PickupStudentProject(string userType, int superUserID, int projectID, int groupID)
		{
			Project currentProject = _readEntities.Get<Project>(p => p.ID == projectID).SingleOrDefault();
			Group currentGroup = currentProject.GetGroup(groupID);
			if (currentProject == null)
				return false;
			string fromUserFirstName = "";
			switch (userType)
			{
				case "Supervisor":
					var currentSupervisor = _userServices.GetCurrentSupervisor(superUserID);
					if (currentSupervisor != null)
					{
						fromUserFirstName = currentSupervisor.Firstname;
						currentGroup.SetSupervisor(currentSupervisor.ID);
					}
					break;
				case "CoSupervisor":
					var currentCoSupervisor = _userServices.GetCurrentCoSupervisor(superUserID);
					if (currentCoSupervisor != null)
					{
						fromUserFirstName = currentCoSupervisor.Firstname;
						currentGroup.SetCoSupervisor(currentCoSupervisor.ID);
						currentGroup.SetSupervisor(currentCoSupervisor.MySupervisorID);
					}
					break;
				case "ExternalSupervisor":
					var currentExternalSupervisor = _userServices.GetCurrentExternalSupervisor(superUserID);
					if (currentExternalSupervisor != null)
					{
						fromUserFirstName = currentExternalSupervisor.Firstname;
						currentGroup.SetCoSupervisor(currentExternalSupervisor.ID);
						currentGroup.SetSupervisor(currentExternalSupervisor.MySupervisorID);
					}
					break;
			}

			//If is full, ApprovedNumber, send Email to all group member, Status->form "InProcess" to "Registered"
			if (currentGroup.Vacancy == 0)
			{
				string approvedNo = getApprovedNumber(currentProject.SubjectID);
				currentProject.ApprovedNumber = approvedNo;
				currentGroup.ApprovedNO = approvedNo + "-" + currentGroup.ID.ToString();
				//student posted project
				currentProject.Status = ProjectStatus.Registered;
			}
			else
				currentProject.Status = ProjectStatus.Processing;

			////this part may need to change
			projectPickedUpSendEmailToGroupMembers(currentProject, currentGroup, fromUserFirstName);

			_writeEntities.Update(currentProject);
			_writeEntities.Save();
			return true;
		}

		#endregion

		#region Project Group

		public bool StudentRequestToJoinAProjectGroup(Student currentStudent, Subject currentEnrolledSubject, ProjectViewModel model)
		{
			if (currentStudent.RequestJoinProjectGroup(model.ID, currentEnrolledSubject.TermID, currentEnrolledSubject.ID, model.GroupID, model.UserID))
			{
				_writeEntities.Update(currentStudent);
				_writeEntities.Save();

				string fromUserName = _readEntities.Get<User>(a => a.ID == currentStudent.ID).FirstOrDefault().Firstname;

				//send email to proposer
				var toUser = _readEntities.Get<User>(a => a.ID == model.UserID).FirstOrDefault();
				string toEmailAddress = toUser.Email;
				string toUserName = toUser.Firstname;
				string subject = "A student has sent a request to join your project group!";
				Project currentProject = _readEntities.Get<Project>(p => p.ID == model.ID).SingleOrDefault();
				_commonServices.SendStudentRequestEmail(subject, currentProject, fromUserName, toUserName, toEmailAddress);
				return true;
			}
			return false;
		}

		public List<RequestToJoinGroupViewModel> GetStudentRequestedList(int subjectID, Student currentStudent)
		{
			List<JoinProjectGroup> joinProjectGroups = currentStudent.GetRequestedProjectGroups().Where(a => a.SubjectID == subjectID).ToList();
			var vmList = new List<RequestToJoinGroupViewModel>();
			joinProjectGroups.ForEach(p =>
			{
				var model = _mapper.Map<JoinProjectGroup, RequestToJoinGroupViewModel>(p);
				model.ProposerID = p.ProposerID;
				var currentGroup = _readEntities.Get<Group>(a => a.ID == p.GroupID).SingleOrDefault();
				if (currentGroup != null)
				{
					model.GroupName = currentGroup.Name;
					model.JoinedStudents = currentGroup.GetStudents();
				}
				var currentProject = _readEntities.Get<Project>(a => a.ID == p.ProjectID).SingleOrDefault();
				if (currentProject != null)
				{
					model.ProjectTitle = currentProject.Title;
					vmList.Add(model);
				}
			});
			return vmList;
		}

		public List<GroupViewModel> GetCurrentSemesterManagedGroupsVMList(string userType, int userID)
		{
			Term currentTerm = _commonServices.GetCurrentOpenTerm();
			if (currentTerm == null)
				return null;
			List<GroupViewModel> groupsVM = null;
			switch (userType)
			{
				case "Supervisor":
					Supervisor currentSupervisor = _readEntities.Get<Supervisor>(a => a.ID == userID).SingleOrDefault();
					groupsVM = getCurrentSemesterManagedGroupsVMListBySupervisor(currentSupervisor, currentTerm);
					break;
				case "CoSupervisor":
					CoSupervisor currentCoSupervisor = _readEntities.Get<CoSupervisor>(a => a.ID == userID).SingleOrDefault();
					groupsVM = getCurrentSemesterManagedGroupsVMListByCoSupervisor(currentCoSupervisor, currentTerm);
					break;
				case "ExternalSupervisor":
					ExternalSupervisor currentExtSupervisor = _readEntities.Get<ExternalSupervisor>(a => a.ID == userID).SingleOrDefault();
					groupsVM = getCurrentSemesterManagedGroupsVMListByExtSupervisor(currentExtSupervisor, currentTerm);
					break;
			}
			return groupsVM;
		}

		public List<GroupViewModel> GetCurrentSubjectGroupsVMList(Subject currentEnrolledSubject)
		{
			//used for coordinator
			IEnumerable<Group> currentGroups = getCurrentSubjectGroups(currentEnrolledSubject);
			if (currentGroups == null)
				return null;

			List<GroupViewModel> groupsVM = new List<GroupViewModel>();
			currentGroups.ToList().ForEach(g =>
						{
							GroupViewModel model = _mapper.Map<Group, GroupViewModel>(g);
							model.JoinedStudents = g.GetStudents();
							model.PostedReports = g.GetReports();
							model.PublisherID = _readEntities.Get<Project>(p => p.ID == g.ProjectID).SingleOrDefault().UserID;
							groupsVM.Add(model);
						});

			return groupsVM;
		}

		public List<GroupViewModel> GetCurrentSubjectStudentJoinedGroupsVMList(Subject currentEnrolledSubject, Student currentStudent)
		{
			IEnumerable<Group> currentGroups = getCurrentSubjectGroups(currentEnrolledSubject);
			if (currentGroups == null)
				return null;
			List<GroupViewModel> groupsVM = new List<GroupViewModel>();
			foreach (var group in currentGroups)
			{
				var myJoinedGroup = currentStudent.GetGroup(group.ID);
				if (myJoinedGroup != null)
				{
					GroupViewModel model = _mapper.Map<Group, GroupViewModel>(group);
					model.JoinedStudents = group.GetStudents();
					model.PostedReports = group.GetReports();
					Project project = _readEntities.Get<Project>(p => p.ID == group.ProjectID).SingleOrDefault();
					if (project != null)
						model.PublisherID = project.UserID;
					groupsVM.Add(model);
				}
			}

			return groupsVM;
		}

		public bool StudentWithdrawGroup(Student currentStudent, int projectID, int groupID)
		{
			JoinProjectGroup requestedProjectGroup = _readEntities.Get<JoinProjectGroup>
				 (j => j.ProjectID == projectID && j.StudentID == currentStudent.ID && j.GroupID == groupID).SingleOrDefault();
			if (requestedProjectGroup == null)
				return false;
			Group group = currentStudent.GetGroup(groupID);
			if (group == null)
				return false;

			currentStudent.WithdrawGroup(group);   //student exit group
			group.DeleteStudent(currentStudent);   //group delete student
			requestedProjectGroup.Quit();          //set JoinProjectGroup
			Student newLeader = group.GetStudents().FirstOrDefault();
			if (newLeader != null)
				group.SetLeader(newLeader.ID);       //set new leader

			_writeEntities.Update(requestedProjectGroup);
			_writeEntities.Update(currentStudent);
			_writeEntities.Save();

			//***need to send Email to members include supervisor

			return true;
		}

		public void UpdateGroupInforByStudent(Student student, GroupViewModel model)
		{
			Group group = student.GetGroup(model.ID);
			group.Name = model.Name;
			group.Description = model.Description;
			_writeEntities.Update(group);
			_writeEntities.Save();
		}

		public void UploadReportByStudent(Student student, ReportViewModel model)
		{
			Report report = new Report(model.GroupID);
			report = _mapper.Map(model, report);
			Group group = student.GetGroup(model.GroupID);
			if (group != null)
			{
				group.AddReport(report);
				_writeEntities.Update(student);
				_writeEntities.Save();
			}
		}

		public void UpdateReportByStudent(Student student, ReportViewModel model)
		{
			Group group = student.GetGroup(model.GroupID);
			Report report = group.GetReport(model.ID);
			if (report != null)
			{
				report = _mapper.Map(model, report);
				group.UpdateReport(report);
				_writeEntities.Update(student);
				_writeEntities.Save();
			}
		}

		public void DeleteReportByStudent(Student student, int groupID, int reportID)
		{
			Group group = student.GetGroup(groupID);
			Report report = group.GetReport(reportID);
			if (report != null)
			{
				group.DeleteReport(report);
				_writeEntities.Delete(report);
				_writeEntities.Update(student);
				_writeEntities.Save();
			}
		}

		#endregion

		#region Report

		public ReportCommentViewModel GetReportCommentVM(string userType, int superUserID, int projectID, int groupID, int reportID)
		{
			Group group = getSupervisorsManagedGroup(userType, superUserID, groupID);
			ReportCommentViewModel model = _mapper.Map<Group, ReportCommentViewModel>(group);
			model.ReportID = reportID;
			return model;
		}

		public bool AddGroupCommentBySupervisors(string userType, int superUserID, ReportCommentViewModel model)
		{
			Group group = getSupervisorsManagedGroup(userType, superUserID, model.ID);

			if (group == null)
				return false;
			Report report = group.GetReport(model.ReportID);
			if (report == null)
				return false;

			if (report.Comments == null)
				report.Comments = model.Comments + " (" + userType + superUserID.ToString() + ")";
			else

				report.Comments = report.Comments + "\n" + model.Comments + " (" + userType + superUserID.ToString() + ")";
			_writeEntities.Update(report);
			_writeEntities.Save();
			return true;
		}

		#endregion

		#region Process Request

		public bool HasSentRequest(int studentID, int projectID, int groupID)
		{
			JoinProjectGroup requestedProjectGroup = _readEntities.Get<JoinProjectGroup>
						(j => j.ProjectID == projectID && j.GroupID == groupID && j.StudentID == studentID).FirstOrDefault();
			if (requestedProjectGroup == null)
				return false;
			if (requestedProjectGroup.RequestStatus == RequestStatus.Quit || requestedProjectGroup.RequestStatus == RequestStatus.Withdraw)
				return false;
			return true;
		}

		public bool ProcessStudentJoinGroupRequirement(int currentUserID, int studentID, int projectID, int groupID, string actionWord)
		{
			bool isNewGroup = false;
			//groupID==0: supervisor posted project, have not create a group 
			string subject = "";
			User fromUser = _readEntities.Get<User>(u => u.ID == currentUserID).FirstOrDefault();
			if (fromUser == null)
				return false;
			Project project = fromUser.GetProject(projectID);
			if (project == null)
				return false;
			//	Group currentGroup = _readEntities.Get<Group>(a => a.ID == groupID).SingleOrDefault();
			Group currentGroup = project.GetGroup(groupID);
			Student targetUser = _readEntities.Get<Student>(s => s.ID == studentID).FirstOrDefault();
			JoinProjectGroup requestedProjectGroup = _readEntities.Get<JoinProjectGroup>
			(j => j.ProjectID == projectID && j.ProposerID == currentUserID
				&& j.GroupID == groupID && j.StudentID == studentID
				&& j.RequestStatus != RequestStatus.Withdraw && j.RequestStatus != RequestStatus.Quit).SingleOrDefault();

			if (requestedProjectGroup == null)
				return false;

			switch (actionWord)
			{
				case "accept":
					requestedProjectGroup.Accept();
					subject = "Your request has accepted !";
					_commonServices.SendAcceptedRequestEmail(subject, project, fromUser.Firstname, targetUser.Firstname, targetUser.Email);
					break;
				case "reject":
					requestedProjectGroup.Reject();
					subject = "Your request has rejected !";
					_commonServices.SendRejectedRequestEmail(subject, project, fromUser.Firstname, targetUser.Firstname, targetUser.Email);
					break;
				case "withdraw":
					requestedProjectGroup.Withdraw();
					subject = "Student has withdrew a project !";
					studentJoinProjectSendEmailToProposer(project, fromUser.Firstname, subject);
					break;
				case "register":
					//The last studen join the group and have a supervisor, add approved number
					if (currentGroup.Vacancy == 1 && currentGroup.SupervisorID != null)
					{
						string approvedNo = getApprovedNumber(project.SubjectID);
						currentGroup.ApprovedNO = approvedNo + "-" + currentGroup.ID.ToString();
						project.ApprovedNumber = approvedNo;
						if (project.PublisherType == "Student")
							project.Status = ProjectStatus.Registered;

						if (project.GetGroups().Count() == project.MaxGroupNumber)
							project.Status = ProjectStatus.Registered;
					}

					//check current group size
					if (currentGroup.Vacancy == 0 && currentGroup.Status == GroupStatus.Full)
					{
						//The project posted by student that cannot create second group
						if (project.PublisherType == "Student")
							return false;

						if (project.GetGroups().Count() >= project.MaxGroupNumber)
							return false;

						//add a new group to the project which supervisor posted
						Group newGroup = createNewGroup(project, currentGroup, targetUser);
						newGroup.AddStudent(targetUser);   //group add a student
						newGroup.SetLeader(targetUser.ID); //set group leader
						targetUser.EnrolGroup(newGroup);   //student join a group
						project.AddGroup(newGroup);        //project add a group
						_writeEntities.Update(fromUser);
						_writeEntities.Update(targetUser);
						isNewGroup = true;
					}
					else
					{
						if (currentGroup.LeaderID == null)
							currentGroup.SetLeader(targetUser.ID);

						currentGroup.AddStudent(targetUser);   //group add a student
						targetUser.EnrolGroup(currentGroup);   //student join a group
						_writeEntities.Update(currentGroup);
						_writeEntities.Update(targetUser);
					}
					requestedProjectGroup.Register();       //set JoinProjectGroup status
					subject = "Student has confirmed to join a project !";
					studentJoinProjectSendEmailToProposer(project, fromUser.Firstname, subject);
					break;
			}
			_writeEntities.Update(requestedProjectGroup);
			_writeEntities.Save();

			// change groupID of JoinProjectGroup 
			if (isNewGroup)
			{
				Group enrolledGroup = targetUser.GetEnrolledGroups().Where(a => a.ProjectID == projectID).FirstOrDefault();
				if (enrolledGroup != null)
					requestedProjectGroup.GroupID = enrolledGroup.ID;
				_writeEntities.Update(requestedProjectGroup);
				_writeEntities.Save();
			}

			return true;
		}

		public List<RequestToJoinGroupViewModel> GetMyPostedProjectJoinGroupRequestVMList(int currentEnrolledSubjectID, int currentUserID)
		{
			return _dbData.GetMyPostedProjectRequestedList(currentEnrolledSubjectID, currentUserID);
		}

		public List<RequestToJoinGroupViewModel> GetCurrentTermMyPostedProjectJoinGroupRequestVMList(int currentTermID, int currentUserID)
		{
			//supervisor only
			return _dbData.GetCurrentTermMyPostedProjectRequestedList(currentTermID, currentUserID);
		}

		public void SendInvitationToStudent(int projectID, int userID)
		{
			Project currentProject = _readEntities.Get<Project>(p => p.ID == projectID).SingleOrDefault();
			User proposer = _readEntities.Get<User>(u => u.ID == currentProject.UserID).SingleOrDefault();

			Student student = _readEntities.Get<Student>(s => s.ID == userID).SingleOrDefault();
			string subject = "An invitation about join a project group!";
			_commonServices.ProjectInvitationEmailToStudent(subject, currentProject, proposer.Firstname, student.Firstname, student.Email);
		}

		public void SendInvitationToSupervisor(int projectID, int userID)
		{
			Project currentProject = _readEntities.Get<Project>(p => p.ID == projectID).SingleOrDefault();
			User proposer = _readEntities.Get<User>(u => u.ID == currentProject.UserID).SingleOrDefault();

			User supervisor = _readEntities.Get<User>(s => s.ID == userID).SingleOrDefault();
			string subject = "An invitation from a student about manage a project group!";
			_commonServices.ProjectInvitationEmailToSupervisor(subject, currentProject, proposer.Firstname, supervisor.Firstname, supervisor.Email);
		}

		#endregion

		#region Co-Supervisor

		public List<MyCoSupervisorViewModel> GetMyCoSupervisorVMList(int currentSupervisorID)
		{
			List<CoSupervisor> myCoSupervisors = _commonServices.GetCurrentSemesterCoSupervisors().Where(c => c.MySupervisorID == currentSupervisorID).OrderBy(a => a.ID).ToList();
			List<MyCoSupervisorViewModel> myCoSupervisorVMList = new List<MyCoSupervisorViewModel>();
			foreach (var coSupervisor in myCoSupervisors)
			{
				MyCoSupervisorViewModel myCoSupervisorVM = new MyCoSupervisorViewModel();
				myCoSupervisorVM = _mapper.Map<CoSupervisor, MyCoSupervisorViewModel>(coSupervisor);
				myCoSupervisorVMList.Add(myCoSupervisorVM);
			}
			return myCoSupervisorVMList;
		}

		public List<ProjectPoolEditModel> GetMyCoSupervisorProjectPoolVMList(int coSupervisorID)
		{
			CoSupervisor currentCoSupervisor = _readEntities.Get<CoSupervisor>(a => a.ID == coSupervisorID).SingleOrDefault();
			if (currentCoSupervisor == null)
				return null;
			List<ProjectPoolEditModel> projectPoolsVM = new List<ProjectPoolEditModel>();
			List<ProjectPool> projectPools = new List<ProjectPool>();
			projectPools = currentCoSupervisor.GetProjectPools().ToList();
			projectPools.ForEach(p =>
			{
				ProjectPoolEditModel model = _mapper.Map<ProjectPool, ProjectPoolEditModel>(p);
				projectPoolsVM.Add(model);
			});
			return projectPoolsVM.OrderByDescending(s => s.ID).ToList();
		}

		public List<ProjectViewModel> GetMyCoSupervisorPublishedPorjectsVMList(int coSupervisorID)
		{
			CoSupervisor currentCoSupervisor = _readEntities.Get<CoSupervisor>(a => a.ID == coSupervisorID).SingleOrDefault();
			if (currentCoSupervisor == null)
				return null;

			List<ProjectViewModel> projectPoolsVM = new List<ProjectViewModel>();
			List<Project> projects = new List<Project>();
			projects = currentCoSupervisor.GetProjects().OrderBy(a => a.SubjectID).ToList();
			projects.ForEach(p =>
			{
				ProjectViewModel model = _mapper.Map<Project, ProjectViewModel>(p);
				model.ProjectGroups = p.GetGroups();
				projectPoolsVM.Add(model);
			});
			return projectPoolsVM.OrderByDescending(s => s.ID).ToList();
		}

		#endregion

		#region DynamicViewBag

		public DynamicViewBag GetRequestedCounterModel(int currentUserID, string userType)
		{
			int requestedJoinGroupCounter = _readEntities.Get<JoinProjectGroup>(a => a.RequestStatus == RequestStatus.Requested && a.ProposerID == currentUserID).Count();
			List<Project> projects = _readEntities.Get<Project>(a => a.PublisherType == "Student").ToList();
			int studentPostedProjectNoSupervisorCounter = 0;
			int requestStatusCounter = 0;
			if (userType == "Student")
				requestStatusCounter = _readEntities.Get<JoinProjectGroup>(a => a.StudentID == currentUserID && (a.RequestStatus == RequestStatus.Accepted || a.RequestStatus == RequestStatus.Rejected)).Count();
			else
			{
				foreach (var project in projects)
				{
					if (project.GetGroups().SingleOrDefault().SupervisorID == null)
						studentPostedProjectNoSupervisorCounter++;
				}
			}
			DynamicViewBag model = new DynamicViewBag();
			model.AddValue("RequestedJoinGroupCounter", requestedJoinGroupCounter);
			model.AddValue("StudentPostedProjectNoSupervisorCounter", studentPostedProjectNoSupervisorCounter);
			model.AddValue("RequestStatusCounter", requestStatusCounter);
			return model;
		}

		#endregion

		#region Coordinator action

		public void RemoveSupervisor(int supervisorID, int projectID)
		{
			Group group = _readEntities.Get<Group>(a => a.ProjectID == projectID).SingleOrDefault();
			group.SupervisorID = null;
			group.ExtSupervisorID = null;
			group.CoSupervisorID = null;
			group.ApprovedNO = null;
			_writeEntities.Update(group);

			Project project = _readEntities.Get<Project>(a => a.ID == projectID).SingleOrDefault();
			if (group.GetStudents().Count() == group.Size)
				project.Status = ProjectStatus.Full;
			else
				project.Status = ProjectStatus.Pending;
			_writeEntities.Update(project);
			_writeEntities.Save();
		}

		public void GradeGroupByCoordinator(int coordinatorID, GradeGroupViewModel model)
		{
			Group group = _readEntities.Get<Group>(a => a.ID == model.ID).SingleOrDefault();
			group.Grade = (Grade)model.Grade;
			group.Comments = group.Comments + model.Comments + " [" + coordinatorID.ToString() + "] \n";
			group.CommentDate = DateTime.Now;
			_writeEntities.Update(group);
			_writeEntities.Save();
		}

		#endregion

		#region Send Email

		private void studentJoinProjectSendEmailToProposer(Project currentProject, string fromUserName, string subject)
		{
			var proposer = _readEntities.Get<User>(a => a.ID == currentProject.UserID).FirstOrDefault();
			if (proposer != null)
			{
				_commonServices.SendEmailToProposer(subject, currentProject, fromUserName, proposer.Firstname, proposer.Email);
			}
		}

		private void studentJoinGroupSendEmailToGroupMembers(Project currentProject, Group currentGroup, string fromUserName, string subject)
		{
			var toEmailAddress = "";

			var currentUser = _readEntities.Get<User>(a => a.ID == currentProject.UserID).SingleOrDefault();
			if (currentUser != null)
			{
				toEmailAddress = currentUser.Email;
				//	this._commonServices.SendEmail(emailSubject, emailContent, userEmail);
				_commonServices.SendEmailToProjectGroup(subject, currentProject, fromUserName, currentUser.Firstname, toEmailAddress);
			}

			//send to group members
			foreach (var item in currentGroup.GetStudents())
			{
				toEmailAddress = item.Email;
				//	this._commonServices.SendEmail(emailSubject, emailContent, toEmailAddress);

				_commonServices.SendEmailToProjectGroup(subject, currentProject, fromUserName, item.Firstname, toEmailAddress);
			}
		}

		private void projectPickedUpSendEmailToGroupMembers(Project currentProject, Group currentGroup, string fromUserName)
		{
			string subject = "Your project has picked up by a supervisor!";
			//send to group members
			foreach (var item in currentGroup.GetStudents())
			{
				var toEmailAddress = item.Email;
				_commonServices.ProjectHasPickedUpEmail(subject, currentProject, fromUserName, item.Firstname, toEmailAddress);
			}
		}

		#endregion

		#region Helper

		private List<GroupViewModel> getCurrentSemesterManagedGroupsVMListBySupervisor(Supervisor currentSupervisor, Term currentTerm)
		{
			List<Subject> subjects = currentTerm.GetSubjects();
			IEnumerable<GroupViewModel> currentGroups = null;
			int i = 0;
			foreach (var subject in subjects)
			{
				if (i == 0)
                {
                    currentGroups = GetCurrentSubjectGroupsVMList(subject);
                    if (currentGroups!=null)   //edit on 26/07/2018
                        currentGroups=currentGroups.Where(a => a.SupervisorID == currentSupervisor.ID);
                }
				else
				{
					var nextGroups = GetCurrentSubjectGroupsVMList(subject);
					if (nextGroups != null)
						currentGroups = currentGroups.Concat(nextGroups.Where(a => a.SupervisorID == currentSupervisor.ID));
				}
				i++;
			}
            if (currentGroups == null)
                return null;
            return currentGroups.ToList();
		}

		private List<GroupViewModel> getCurrentSemesterManagedGroupsVMListByCoSupervisor(CoSupervisor currentCoSupervisor, Term currentTerm)
		{
			List<Subject> subjects = currentCoSupervisor.GetEnrolledSubjects().Where(a => a.TermID == currentTerm.ID).ToList();
			IEnumerable<GroupViewModel> currentGroups = null;
			int i = 0;
			foreach (var subject in subjects)
			{
				if (i == 0)
					currentGroups = GetCurrentSubjectGroupsVMList(subject).Where(a => a.CoSupervisorID == currentCoSupervisor.ID);
				else
				{
					var nextGroups = GetCurrentSubjectGroupsVMList(subject);
					if (nextGroups != null)
						currentGroups = currentGroups.Concat(nextGroups.Where(a => a.CoSupervisorID == currentCoSupervisor.ID));
				}
				i++;
			}
			return currentGroups.ToList();
		}

		private List<GroupViewModel> getCurrentSemesterManagedGroupsVMListByExtSupervisor(ExternalSupervisor currentExtSupervisor, Term currentTerm)
		{
			List<Subject> subjects = currentExtSupervisor.GetEnrolledSubjects().Where(a => a.TermID == currentTerm.ID).ToList();
			IEnumerable<GroupViewModel> currentGroups = null;
			int i = 0;
			foreach (var subject in subjects)
			{
				if (i == 0)
					currentGroups = GetCurrentSubjectGroupsVMList(subject).Where(a => a.ExtSupervisorID == currentExtSupervisor.ID);
				else
				{
					var nextGroups = GetCurrentSubjectGroupsVMList(subject);
					if (nextGroups != null)
						currentGroups = currentGroups.Concat(nextGroups.Where(a => a.ExtSupervisorID == currentExtSupervisor.ID));
				}
				i++;
			}
			return currentGroups.ToList();
		}

		private Group getSupervisorsManagedGroup(string userType, int superUserID, int groupID)
		{
			Group group = null;
			switch (userType)
			{
				case "Supervisor":
					var currentSupervisor = _userServices.GetCurrentSupervisor(superUserID);
					if (currentSupervisor != null)
						group = currentSupervisor.GetGroup(groupID);
					break;
				case "CoSupervisor":
					group = _readEntities.Get<Group>(g => g.ID == groupID && g.CoSupervisorID == superUserID).SingleOrDefault();
					break;
				case "ExternalSupervisor":
					group = _readEntities.Get<Group>(g => g.ID == groupID && g.ExtSupervisorID == superUserID).SingleOrDefault();
					break;
			}
			return group;
		}

		private Group createNewGroup(Project project, Group currentGroup, Student targetUser)
		{
			//add a new group to the project which supervisor posted
			Group newGroup = new Group(project.ID, "Group " + (project.GetGroups().Count() + 1).ToString(), "Group description", project.GroupSize);
			if (currentGroup.SupervisorID != null)
				newGroup.SetSupervisor((int)currentGroup.SupervisorID);    //set supervisor					
			if (currentGroup.CoSupervisorID != null)
				newGroup.SetCoSupervisor((int)currentGroup.CoSupervisorID);
			if (currentGroup.ExtSupervisorID != null)
				newGroup.SetExternalSupervisor((int)currentGroup.ExtSupervisorID);

			return newGroup;
		}

		private string getApprovedNumber(int subjectID)
		{
			string approvedNumber = "";
			var currentSubject = _readEntities.Get<Subject>(a => a.ID == subjectID).SingleOrDefault();
			if (currentSubject != null)
			{
				string termName = "";
				var currentTerm = _readEntities.Get<Term>(t => t.ID == currentSubject.TermID).SingleOrDefault();
				if (currentTerm != null)
					termName = currentTerm.TermName;
				var currentTermProjects = _readEntities.Get<Project>().Where(a => a.SubjectID == currentSubject.ID && a.ApprovedNumber != null);
				int count = 0;
				if (currentTermProjects != null)
					count = currentTermProjects.Count();
				count++;
				approvedNumber = termName + currentSubject.SubjectName + "-" + count.ToString();
			}
			return approvedNumber;
		}

		private Project addSelectKeywordToProject(Project project, int[] selectedKeywordIDs)
		{
			//add selected keyword to project 
			if (selectedKeywordIDs != null)
			{
				//add new selected
				foreach (int keywordID in selectedKeywordIDs)
				{
					var projectKeyword = project.GetProjectKeywordByID(keywordID);
					if (projectKeyword == null)
					{
						ProjectKeyword newProjectKeyword = new ProjectKeyword(project.ID, keywordID);
						project.AddProjectKeyword(newProjectKeyword);
					}
				}
			}
			return project;
		}

		private void delectUnselectItems(int[] selectedKeywordIDs, Project project)
		{
			var newProjectKeywords = _readEntities.Get<ProjectKeyword>(a => a.ProjectID == project.ID);
			if (newProjectKeywords != null)
			{
				//delete unselect items
				foreach (var element in newProjectKeywords)
				{
					bool find = false;
					if (selectedKeywordIDs != null)
					{
						foreach (int selectId in selectedKeywordIDs)
						{
							if (element.KeywordID == selectId)
							{
								find = true;
								break;
							}
						}
					}
					if (!find)
					{
						_writeEntities.Delete(element);
						_writeEntities.Update(project);
						_writeEntities.Save();
					}
				}
			}
		}

		//for superviser, co-supervisor and ext-supervisor
		private Project updateProjectStatus(Project project, int newGroupSize)
		{
			bool isFUll = true;
			foreach (var group in project.GetGroups())
			{
				project = updateProjectGroupStatus(project, group, newGroupSize);
				if (group.GetStudents().Count() != group.Size)
					isFUll = false;
			}

			//close project when group number equal max group number
			if (isFUll)
			{
				if (string.IsNullOrEmpty(project.ApprovedNumber))
					project.ApprovedNumber = getApprovedNumber(project.SubjectID);
				project.Status = ProjectStatus.Registered;
			}
			return project;
		}

		private Project updateProjectGroupStatus(Project project, Group group, int newGroupSize)
		{
			int joinedStudents = group.GetStudents().Count();
			if (newGroupSize > joinedStudents)
			{
				//the group size does not be changed when the group has fulled and project posted by studnet
				if ((project.PublisherType == "Student" && (!string.IsNullOrEmpty(group.ApprovedNO))))
				{
					project.GroupSize = joinedStudents;
				}
				else
				{
					group.Size = newGroupSize;
					group.Vacancy = newGroupSize - joinedStudents;
					group.Status = GroupStatus.Avaliable;
					project.Status = ProjectStatus.Processing;
				}
			}
			else if (newGroupSize == joinedStudents)
			{
				string approvedNo;
				if (string.IsNullOrEmpty(project.ApprovedNumber))
					approvedNo = getApprovedNumber(project.SubjectID);
				else
					approvedNo = project.ApprovedNumber;

				//for student posted project only
				if (project.PublisherType == "Student" && group.SupervisorID != null)
				{
					project.ApprovedNumber = approvedNo;
					project.Status = ProjectStatus.Registered;
				}

				group.ApprovedNO = approvedNo + "-" + group.ID.ToString();
				group.Status = GroupStatus.Full;
				group.Size = newGroupSize;
				group.Vacancy = 0;
			}
			project.UpdateGroup(group);
			return project;
		}

		private List<ProjectViewModel> mapProjectPostedByStudentToVMList(IEnumerable<Project> projects)
		{
			projects = projects.Where(p => p.PublisherType == "Student").ToList();
			List<ProjectViewModel> projectsVM = new List<ProjectViewModel>();
			projects.ToList().ForEach(p =>
				{
					ProjectViewModel model = _mapper.Map<Project, ProjectViewModel>(p);
					// model.JoinedStudents = p.GetGroups().SingleOrDefault().GetStudents().Count();
					model.ProjectGroups = p.GetGroups();
					model.PublisherType = p.PublisherType; //_userServices.GetUserRoleByID(p.UserID);
					projectsVM.Add(model);
				});

			return projectsVM;
		}

		private IEnumerable<Group> getCurrentSubjectGroups(Subject currentEnrolledSubject)
		{
			List<Project> projects = currentEnrolledSubject.GetProjects();
			IEnumerable<Group> currentGroups = null;
			int i = 0;
			foreach (var project in projects)
			{
				if (i == 0)
					currentGroups = project.GetGroups();
				else
				{
					var nextGroups = project.GetGroups();
					if (nextGroups != null)
						currentGroups = currentGroups.Concat(nextGroups);
				}
				i++;
			}
			return currentGroups;
		}

		#endregion
	}
}

