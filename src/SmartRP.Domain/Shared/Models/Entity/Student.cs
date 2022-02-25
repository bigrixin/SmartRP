using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRP.Domain
{
	public class Student : User
	{

		#region Properties

		public string StudentID { get; set; }
		public string Degree { get; set; }

		public string PhotoURL { get; set; }
		public float? GPA { get; set; }
		public bool HasJoinedCurrentSubjectProjectGroup { get; set; }
		public int? PreProjectID { get; private set; }
		public virtual ICollection<Subject> Subjects { get; set; }  //many to many
		public virtual ICollection<Group> Groups { get; set; }      //many to many
		public virtual ICollection<JoinProjectGroup> RequestToJoinProjectGroups { get; set; } //one to many

		#endregion

		#region Ctor

		protected Student()
		{
			// required by EF
			HasJoinedCurrentSubjectProjectGroup = false;
			Subjects = new List<Subject>();
			Groups = new List<Group>();
			RequestToJoinProjectGroups = new List<JoinProjectGroup>();
		}

		public Student(string aspNetIdentity) : base(aspNetIdentity)
		{
			HasJoinedCurrentSubjectProjectGroup = false;
			Subjects = new List<Subject>();
			Groups = new List<Group>();
			RequestToJoinProjectGroups = new List<JoinProjectGroup>();
			CreatedAt = DateTime.Now;
			UpdatedAt = DateTime.Now;
		}

		#endregion

		#region Subject

		public List<Subject> GetEnrolledSubjects()
		{
			return Subjects.ToList();
		}

		public Subject FindEnrolledSubjectInTerm(int termID)
		{
			return Subjects.Where(s => s.TermID == termID).SingleOrDefault();
		}

		public Subject GetSubjectByID(int ID)
		{
			return Subjects.Where(k => k.ID == ID).SingleOrDefault();
		}

		public Subject EnrolSubject(Subject subject)
		{
			var existingSubject = Subjects.Where(k => k.ID == subject.ID);
			if (existingSubject.Any())
				return existingSubject.FirstOrDefault();
			Subjects.Add(subject);
			HasJoinedCurrentSubjectProjectGroup = false;
			return subject;
		}

		public void WithdrawSubject(Subject subject)
		{
			var existingSubject = Subjects.Where(k => k.ID == subject.ID).SingleOrDefault();
			if (existingSubject != null)
			{
				HasJoinedCurrentSubjectProjectGroup = false;
				Subjects.Remove(existingSubject);
		  }
		}

		#endregion

		#region Group

		public List<Group> GetEnrolledGroups()
		{
			return Groups.ToList();
		}

		public Group GetGroup(int groupID)
		{
			return Groups.Where(s => s.ID == groupID).SingleOrDefault();
		}

		public Group EnrolGroup(Group group)
		{
			var existingGroup = Groups.Where(k => k.ID == group.ID);
			if (existingGroup.Any())
				return existingGroup.FirstOrDefault();
			Groups.Add(group);
			HasJoinedCurrentSubjectProjectGroup = true;
			return group;
		}

		public void WithdrawGroup(Group group)
		{
			var existingGroup = Groups.Where(k => k.ID == group.ID).SingleOrDefault();
			if (existingGroup != null)
			{
				Groups.Remove(existingGroup);
				HasJoinedCurrentSubjectProjectGroup = false;
			}
		}

		#endregion

		#region JoinProjectGroup

		public List<JoinProjectGroup> GetRequestedProjectGroups()
		{
			return RequestToJoinProjectGroups.ToList();
		}

		public JoinProjectGroup GetRequestedProjectGroup(int groupID)
		{
			return RequestToJoinProjectGroups.Where(s => s.ID == groupID).SingleOrDefault();
		}

		public bool RequestJoinProjectGroup(int projectID, int termID, int subjectID, int groupID, int proposerID)
		{
			var existingJoinProjectGroup = RequestToJoinProjectGroups.Where(k => k.ProjectID == projectID && k.RequestStatus == RequestStatus.Registered);
			if (existingJoinProjectGroup.Any())
				return false;
			existingJoinProjectGroup =RequestToJoinProjectGroups.Where(s => s.StudentID == ID && s.ProjectID == projectID && s.GroupID==groupID && s.RequestStatus==RequestStatus.Requested);
			if (existingJoinProjectGroup.Any())
				return false;

			JoinProjectGroup joinProjectGroup = new JoinProjectGroup(ID, projectID, termID, subjectID, groupID, proposerID);
			RequestToJoinProjectGroups.Add(joinProjectGroup);
			return joinProjectGroup.Request();
		}

		public bool WithdrawJoinProjectGroup(JoinProjectGroup group)
		{
			var existingJoinProjectGroup = GetRequestedProjectGroup(group.ID);
			if (existingJoinProjectGroup != null)
				return existingJoinProjectGroup.Withdraw();
			return false;
		}

		public bool ConfirmToJoinProjectGroup(JoinProjectGroup group)
		{
			var existingJoinProjectGroup = GetRequestedProjectGroup(group.ID);
			if (existingJoinProjectGroup != null)
				return existingJoinProjectGroup.Register();
			return false;
		}

		public bool AcceptStudentJoinProjectGroup(int studentID, int groupID)
		{
			var existingJoinProjectGroup = RequestToJoinProjectGroups.Where(s => s.ID == groupID && s.StudentID == studentID).SingleOrDefault();
			if (existingJoinProjectGroup != null)
				return existingJoinProjectGroup.Accept();
			return false;
		}

		public bool RejectStudentJoinProjectGroup(int studentID, int groupID)
		{
			var existingJoinProjectGroup = RequestToJoinProjectGroups.Where(s => s.ID == groupID && s.StudentID == studentID).SingleOrDefault();
			if (existingJoinProjectGroup != null)
				return existingJoinProjectGroup.Reject();
			return false;
		}
		#endregion
	}
}