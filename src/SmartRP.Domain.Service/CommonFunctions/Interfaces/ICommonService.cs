using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SmartRP.Domain.Service
{
	public interface ICommonService
	{
		List<Keyword> GetKeywordList();
		List<Keyword> GetUserSelectedKeywordsByUserID(int userID);
		List<Keyword> GetUserSelectedKeywords(int[] selectedKeywordIDs);

		List<Keyword> GetProjectSelectedKeywordsByProjectID(int projectID);

		void UpdateProfile(User user, int[] selectedKeywordIDs);
		List<SelectListItem> GetSupervisorSelectList();
		DateTime GetTermStartTime(Session session, int year);
		DateTime GetTermEndTime(Session session, int year);

		List<SubjectNameModel> GetInitialSubjectNames(int termID);
		List<Subject> GetSubjectsFromModel(List<SubjectNameModel> subjectNames, int termID);
		List<Term> GetTermList();
		SelectList GetTermSelectList();

		SelectList GetTermSubjectSelectList(int termID);
		Term GetCurrentOpenTerm();

		IEnumerable<CoSupervisor> GetCurrentSemesterCoSupervisors();
		IEnumerable<Student> GetCurrentSemesterStudents();
		IEnumerable<Student> GetCurrentSubjectStudents(int subjectID);
		List<Subject> GetEnrolledTermSubjects(int userID, string userType);
		List<TermSubjectModel> GetEnrolledTermSubjectsVMList(int userID, string userType);

		Subject UserEnrolSubject(string userType, EnrolSubjectViewModel model);

		//List<Supervisor> GetAllSupervisorList();

		//Term GetEnroledTerm(int termId);

	  SubjectName GetSubjectNameByID(int subjectID);
		Term GetSemesterBySubjectID(int subjectID);
		IEnumerable<Student> GetEnrolledStudentsBySemester(Term currentTerm);
		Subject GetCurrentStudentEnrolledSubject(Student currentStudent);
		Subject GetCurrentCoSupervisorEnrolledSubject(CoSupervisor currentCoSupervisor);
		Subject GetCurrentExternalSupervisorEnrolledSubject(ExternalSupervisor currentExternalSupervisor);



		void SendStudentRequestEmail(string subject, Project project, string fromUserName, string toUserName, string toEmailAddress);
		void SendAcceptedRequestEmail(string subject, Project project, string fromUserName, string toUserName, string toEmailAddress);
		void SendRejectedRequestEmail(string subject, Project project, string fromUserName, string toUserName, string toEmailAddress);
		void SendEmailToProjectGroup(string subject, Project project, string fromUserName, string toUserName, string toEmailAddress);
		void SendEmailToProposer(string subject, Project project, string fromUserName, string toUserName, string toEmailAddress);
		void ProjectHasPickedUpEmail(string subject, Project project, string fromUserName, string toUserName, string toEmailAddress);
    void ProjectInvitationEmailToStudent(string subject, Project project, string fromUserName, string toUserName, string toEmailAddress);
		void ProjectInvitationEmailToSupervisor(string subject, Project project, string fromUserName, string toUserName, string toEmailAddress);

	}
}