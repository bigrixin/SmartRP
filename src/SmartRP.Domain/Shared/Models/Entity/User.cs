using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRP.Domain
{
	public abstract class User
	{

		#region Properties

		public int ID { get; set; }
		public string Firstname { get; set; }
		public string Lastname { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Introduction { get; set; }
		public string ResumeURL { get; set; }
		public string SuggestedKeyword { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public string LoginIdentityID { get; private set; }

		public virtual ICollection<Project> Projects { get; set; }
		public virtual ICollection<UserKeyword> UserKeywords { get; set; }

		#endregion

		#region Ctor

		protected User()
		{
			// required by EF
			Projects = new List<Project>();
			UserKeywords = new List<UserKeyword>();
		}

		public User(string loginIdentityID)
		{
			LoginIdentityID = loginIdentityID;
			Projects = new List<Project>();
			UserKeywords = new List<UserKeyword>();
			CreatedAt = DateTime.Now;
			UpdatedAt = DateTime.Now;
		}

		#endregion

		#region Project

		public List<Project> GetProjects()
		{
			return Projects.ToList();
		}

		public Project GetProject(int projectID)
		{
			return Projects.Where(a => a.ID == projectID).SingleOrDefault();
		}

		public Project AddProject(Project project)
		{
			var existingProject = Projects.Where(k => k.ID == project.ID);
			if (existingProject.Any())
				return existingProject.FirstOrDefault();
			Projects.Add(project);
			return project;
		}

		public Project UpdateProject(Project project)
		{
			var existingProject = Projects.Where(k => k.ID == project.ID).SingleOrDefault();
			if (existingProject != null)
			{
				project.UpdatedAt = DateTime.Now;
				Projects.Remove(existingProject);
				Projects.Add(project);
				return project;
			}
			return null;
		}

		public void DeleteProject(Project project)
		{
			var existingProject = Projects.Where(k => k.ID == project.ID).SingleOrDefault();
			if (existingProject != null)
				Projects.Remove(existingProject);
		}

		#endregion

		#region UserKeyword

		public void RemoveUserKeywords()
		{
			var userKeywords= UserKeywords.ToList();
			foreach (var element in userKeywords)
			{
				UserKeywords.Remove(element);
			}
		}

		public virtual List<UserKeyword> GetUserKeywords()
		{
			return UserKeywords.ToList();
		}

		public virtual UserKeyword GetUserKeywordByID(int keywordID)
		{
			return UserKeywords.Where(k => k.KeywordID == keywordID).SingleOrDefault();
		}

		public virtual UserKeyword AddUserKeyword(UserKeyword userKeyword)
		{
			var existingKeyword = UserKeywords.Where(k => k.KeywordID == userKeyword.KeywordID);
			if (existingKeyword.Any())
				return existingKeyword.FirstOrDefault();
			UserKeywords.Add(userKeyword);
			return userKeyword;
		}
 
		public virtual void DeleteUserKeyword(UserKeyword userKeyword)
		{
			var existingKeyword = GetUserKeywordByID(userKeyword.KeywordID);

			if (existingKeyword != null)
				UserKeywords.Remove(existingKeyword);
		}

		#endregion

	}
}