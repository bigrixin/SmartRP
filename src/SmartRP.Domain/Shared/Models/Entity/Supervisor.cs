using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRP.Domain
{
	public class Supervisor : User
	{

		#region Properties

	  public SupervisorType SupervisorType { get; set; }
		public virtual ICollection<Group> Groups { get; set; }
		public virtual ICollection<ProjectPool> ProjectPools { get; set; }
		public virtual ICollection<PreProject> PreProjects { get; set; }
		public virtual ICollection<JoinProjectGroup> RequestToJoinProjectGroups { get; set; } //one to many

		#endregion

		#region Ctor

		protected Supervisor()
		{
			// required by EF
			Groups = new List<Group>();
			ProjectPools = new List<ProjectPool>();
			PreProjects = new List<PreProject>();
			RequestToJoinProjectGroups = new List<JoinProjectGroup>();
		}

		public Supervisor(string aspNetIdentity) : base(aspNetIdentity)
		{
			Groups = new List<Group>();
			ProjectPools = new List<ProjectPool>();
			PreProjects = new List<PreProject>();
			RequestToJoinProjectGroups = new List<JoinProjectGroup>();
			CreatedAt = DateTime.Now;
			UpdatedAt = DateTime.Now;
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
			return group;
		}

		public void WithdrawGroup(Group group)
		{
			var existingGroup = Groups.Where(k => k.ID == group.ID).SingleOrDefault();
			if (existingGroup != null)
				Groups.Remove(existingGroup);
		}

		#endregion

		#region ProjectPool

		public List<ProjectPool> GetProjectPools()
		{
			return ProjectPools.ToList();
		}

		public ProjectPool GetProjectPool(int projectPoolID)
		{
			return ProjectPools.Where(a => a.ID == projectPoolID).SingleOrDefault();
		}

		public ProjectPool AddProjectPool(ProjectPool projectPool)
		{
			var existingProjectPool = ProjectPools.Where(k => k.ID == projectPool.ID);
			if (existingProjectPool.Any())
				return existingProjectPool.FirstOrDefault();
			ProjectPools.Add(projectPool);
			return projectPool;
		}

		public ProjectPool UpdateProjectPool(ProjectPool projectPool)
		{
			var existingProjectPool = ProjectPools.Where(k => k.ID == projectPool.ID).SingleOrDefault();
			if (existingProjectPool != null)
			{
				ProjectPools.Remove(existingProjectPool);
				ProjectPools.Add(projectPool);
				return projectPool;
			}
			return null;
		}

		public void DeleteProjectPool(ProjectPool projectPool)
		{
			var existingProjectPool = ProjectPools.Where(k => k.ID == projectPool.ID).SingleOrDefault();
			if (existingProjectPool != null)
				ProjectPools.Remove(existingProjectPool);
		}

		#endregion

		#region PreProject

		public List<PreProject> GetPreProjects()
		{
			return PreProjects.ToList();
		}

		public PreProject GetPreProject(int prePreProjectID)
		{
			return PreProjects.Where(a => a.ID == prePreProjectID).SingleOrDefault();
		}

		public PreProject AddPreProject(PreProject prePreProject)
		{
			var existingPreProject = PreProjects.Where(k => k.ID == prePreProject.ID);
			if (existingPreProject.Any())
				return existingPreProject.FirstOrDefault();
			PreProjects.Add(prePreProject);
			return prePreProject;
		}

		public PreProject UpdatePreProject(PreProject prePreProject)
		{
			var existingPreProject = PreProjects.Where(k => k.ID == prePreProject.ID).SingleOrDefault();
			if (existingPreProject != null)
			{
				PreProjects.Remove(existingPreProject);
				PreProjects.Add(prePreProject);
				return prePreProject;
			}
			return null;
		}

		public void DeletePreProject(PreProject prePreProject)
		{
			var existingPreProject = PreProjects.Where(k => k.ID == prePreProject.ID).SingleOrDefault();
			if (existingPreProject != null)
				PreProjects.Remove(existingPreProject);
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

		public bool AcceptStudentJoinProjectGroup(int studentID, int groupID)
		{ 
			var existingJoinProjectGroup = RequestToJoinProjectGroups.Where(s => s.ID == groupID && s.StudentID==studentID).SingleOrDefault();
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