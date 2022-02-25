using SmartRP.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRP.Infrastructure.Data
{
	public class DbData
	{
		#region Fields

		public readonly SmartRPDbContext _context;

		#endregion

		#region Ctor

		public DbData(SmartRPDbContext context)
		{
			_context = context;
		}

		#endregion

		#region Helpers

		public List<RequestToJoinGroupViewModel> GetMyPostedProjectRequestedList(int subjectID, int userID)
		{
			var vmList =
			from pj in _context.JoinProjectGroups
			join p in _context.Projects on pj.ProjectID equals p.ID
			join s in _context.Subjects on p.SubjectID equals s.ID
			join t in _context.Terms on s.TermID equals t.ID
			where pj.SubjectID == subjectID && pj.ProposerID == userID

			select new RequestToJoinGroupViewModel
			{
				ID = pj.ProjectID,
				ProposerID = pj.ProposerID,
				GroupID = pj.GroupID,
				ProjectID = pj.ProjectID,
				SemesterName = t.TermName,
				SubjectName = s.SubjectName,
				ProjectTitle = p.Title,

				StudentID = pj.StudentID,
				RequestStatus = pj.RequestStatus,
			};
			return vmList.ToList();
		}

		public List<RequestToJoinGroupViewModel> GetCurrentTermMyPostedProjectRequestedList(int termID, int userID)
		{
			//used by supervisor
			var vmList =
					from pj in _context.JoinProjectGroups
					join p in _context.Projects on pj.ProjectID equals p.ID
					join s in _context.Subjects on p.SubjectID equals s.ID
					join t in _context.Terms on s.TermID equals t.ID
					where (pj.TermID == termID && pj.ProposerID == userID)
					orderby p.SubjectID
					select new RequestToJoinGroupViewModel
					{
						ID = pj.ProjectID,
						ProposerID = pj.ProposerID,
						GroupID = pj.GroupID,
						ProjectID = pj.ProjectID,
						ProjectTitle = p.Title,
						SemesterName = t.TermName,
						SubjectName = s.SubjectName,
						StudentID = pj.StudentID,
						RequestStatus = pj.RequestStatus,
					};
			return vmList.ToList();
		}

		public List<RequestsViewModel> GetStudentsRequestsList(int termId)
		{
			//coordinator use only
			var vmList =
			from j in _context.JoinProjectGroups
			join p in _context.Projects on j.ProjectID equals p.ID
			join u in _context.Users on j.ProposerID equals u.ID
			where (j.RequestStatus == 0)
			orderby u.Email
			select new RequestsViewModel
			{
				ID = j.ProjectID,
				Title = p.Title,
				ProposerName = u.Firstname + " " + u.Lastname,
				Email = u.Email,
				GroupSize = p.GroupSize,
				//		JoinedStudents = p.JoinedStudents,
				StudentID = j.StudentID
			};
			return vmList.ToList();
		}

		public List<MatchKeywordViewModel> FindStudentsMatchProjectKeyword(int projectID)
		{
			var vmList =
				from st in _context.Students
				join uk in _context.UserKeywords on st.ID equals uk.UserID
				join k in _context.Keywords on uk.KeywordID equals k.ID
				join pk in _context.ProjectKeywords on k.ID equals pk.KeywordID
				join p in _context.Projects on pk.ProjectID equals p.ID
				join s in _context.Subjects on p.SubjectID equals s.ID
				orderby st.ID
				where (p.ID == projectID && !st.HasJoinedCurrentSubjectProjectGroup)
				select new MatchKeywordViewModel
				{
					ID = k.ID,
					Title = k.Title,
					Description = k.Description,
					UserType = "Student",
					UserID = st.ID,
					KeywordID = k.ID,
				};
			return vmList.ToList();
		}

	
		public List<MatchKeywordViewModel> FindStudentsMatchProjectKeywordOrderByKeywords(int projectID)
		{
			var vmList =
				from k in _context.Keywords
				join uk in _context.UserKeywords on k.ID equals uk.KeywordID
				join st in _context.Students on uk.UserID equals st.ID
				join pk in _context.ProjectKeywords on k.ID equals pk.KeywordID
				join p in _context.Projects on pk.ProjectID equals p.ID
				join s in _context.Subjects on p.SubjectID equals s.ID
				orderby k.ID
				where (p.ID == projectID && !st.HasJoinedCurrentSubjectProjectGroup)
				select new MatchKeywordViewModel
				{
					ID = k.ID,
					Title = k.Title,
					UserType = "Student",
					UserID = st.ID,
					KeywordID = k.ID,
					Description = k.Description,
					Name = st.Lastname + " " + st.Firstname,
					Email = st.Email,
					Degree = st.Degree
				};
			return vmList.ToList();
		}

		public List<MatchKeywordViewModel> FindSupervisorsMatchProjectKeyword(int projectID)
		{
			var vmList =
				from sup in _context.Supervisors
				join uk in _context.UserKeywords on sup.ID equals uk.UserID
				join k in _context.Keywords on uk.KeywordID equals k.ID
				join pk in _context.ProjectKeywords on k.ID equals pk.KeywordID
				join p in _context.Projects on pk.ProjectID equals p.ID
				orderby sup.ID
				where (p.ID == projectID && sup.SupervisorType!=SupervisorType.External)
				select new MatchKeywordViewModel
				{
					ID = k.ID,
					Title = k.Title,
					UserType = sup.SupervisorType.ToString(),
					UserID = sup.ID
				};
			return vmList.ToList();
		}

		#endregion

	}
}
