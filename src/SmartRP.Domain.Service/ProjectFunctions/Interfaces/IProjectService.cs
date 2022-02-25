
using RazorEngine.Templating;
using System.Collections.Generic;

namespace SmartRP.Domain.Service
{
	public interface IProjectService
	{
		ProjectEditModel MapperProjectToEditModel(Project project, int userID, string userType);
		ProjectViewModel MapperProjectToViewModel(int projectID);
		ProjectPoolEditModel MapperProjectPoolToViewModel(string userType, int userID, int projectID);
		ProjectViewModel MapperProjectGroupToViewModel(int projectID, int groupID);
		GradeGroupViewModel MapperGradeGroupToViewModel(int groupID);
		bool AddProjectToPool(string userType, int userID, ProjectPoolEditModel model);
		bool UpdateProjectOfPool(string userType, int userID, ProjectPoolEditModel model);
		void DeletePostedProjectPool(string userType, int userID, int projectPoolID);

		bool AddPostedProjectBySupervisor(Supervisor currentSupervisor, ProjectEditModel model);
		bool UpdatePostedProjectBySupervisor(int userID, ProjectEditModel model);
		void AddPostedProjectByStudent(Student currentStudent, ProjectEditModel model);
		void UpdatePostedProjectByStudent(Student currentStudent, ProjectEditModel model);
		void DeletePostedProjectGroupByStudent(Student currentStudent, int projectID, int groupID);


		bool AddPostedProject(string userType, int userID, ProjectEditModel model);

		void UpdatePostedProject(int userID, ProjectEditModel model);
		void DeletePostedProjectGroup(int userID, int projectID, int groupID);
		IEnumerable<Project> GetCurrentSubjectProjectList(int subjectID);
		IEnumerable<Project> GetCurrentSemesterProjectList();
		IEnumerable<Project> GetCurrentEnrolledSemesterByCoSupervisorProjectList(int coSupervisorID);
		List<ProjectViewModel> GetProjectPostedByStudents(Subject subject, string userType, int superUserID);

		bool IsCurrentSubjectProject(int subjectID, int projectID);
		bool PickupStudentProject(string userType, int superUserID, int projectID, int groupID);
		bool StudentRequestToJoinAProjectGroup(Student currentStudent, Subject currentEnrolledSubject, ProjectViewModel model);
		List<RequestToJoinGroupViewModel> GetStudentRequestedList(int subjectID, Student currentStudent);
		List<GroupViewModel> GetCurrentSemesterManagedGroupsVMList(string userType, int userID);

		List<GroupViewModel> GetCurrentSubjectGroupsVMList(Subject currentEnrolledSubject);
		List<GroupViewModel> GetCurrentSubjectStudentJoinedGroupsVMList(Subject currentEnrolledSubject, Student currentStudent);
		bool StudentWithdrawGroup(Student currentStudent, int projectID, int groupID);
		void UpdateGroupInforByStudent(Student student, GroupViewModel model);
		void UploadReportByStudent(Student student, ReportViewModel model);
		void UpdateReportByStudent(Student student, ReportViewModel model);
		void DeleteReportByStudent(Student student, int groupID, int reportID);

		ReportCommentViewModel GetReportCommentVM(string userType, int superUserID, int projectID, int groupID, int reportID);
		bool AddGroupCommentBySupervisors(string userType, int superUserID, ReportCommentViewModel model);
		bool HasSentRequest(int studentID, int projectID, int groupID);
		bool ProcessStudentJoinGroupRequirement(int currentUserID, int studentID, int projectID, int groupID, string actionWord);
		List<RequestToJoinGroupViewModel> GetMyPostedProjectJoinGroupRequestVMList(int currentEnrolledSubjectID, int currentUserID);
		List<RequestToJoinGroupViewModel> GetCurrentTermMyPostedProjectJoinGroupRequestVMList(int currentTermID, int currentUserID);
		void SendInvitationToStudent(int projectID, int userID);
		void SendInvitationToSupervisor(int projectID, int userID);

		List<MyCoSupervisorViewModel> GetMyCoSupervisorVMList(int currentSupervisorID);
		List<ProjectPoolEditModel> GetMyCoSupervisorProjectPoolVMList(int coSupervisorID);
		List<ProjectViewModel> GetMyCoSupervisorPublishedPorjectsVMList(int coSupervisorID);

		DynamicViewBag GetRequestedCounterModel(int currentUserID, string userType);
		void RemoveSupervisor(int supervisorID, int projectID);
		void GradeGroupByCoordinator(int coordinatorID, GradeGroupViewModel model);
	}
}