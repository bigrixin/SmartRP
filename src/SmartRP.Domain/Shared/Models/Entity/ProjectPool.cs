using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRP.Domain
{
	public class ProjectPool
	{
		#region Properties

		public int ID { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string DocumentURL { get; set; }
		public string SkillsRequest { get; set; }
		public int GroupSize { get; set; }

		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }

		public int SupervisorID { get; set; }
 
		public virtual ICollection<Reference> References { get; set; }
		public virtual ICollection<ProjectKeyword> ProjectKeywords { get; set; }
		public virtual ICollection<Skill> Skills { get; set; }
		public virtual ICollection<Project> Projects { get; set; }

		#endregion

		#region Ctor

		protected ProjectPool()
		{
			// required by EF
			References = new List<Reference>();
			ProjectKeywords = new List<ProjectKeyword>();
			Skills = new List<Skill>();
			Projects = new List<Project>();
		}

		public ProjectPool(int userID)
		{
			SupervisorID = userID;
			CreatedAt = DateTime.Now;
			UpdatedAt = DateTime.Now;
			References = new List<Reference>();
		  ProjectKeywords = new List<ProjectKeyword>();
			Skills = new List<Skill>();
			Projects = new List<Project>();
		}

		#endregion

		#region Reference

		public List<Reference> GetReferences()
		{
			return References.ToList();
		}

		public Reference AddReference(Reference reference)
		{
			var existingReference = References.Where(k => k.ID == reference.ID);
			if (existingReference.Any())
				return existingReference.FirstOrDefault();
			References.Add(reference);
			return reference;
		}

		public Reference UpdateReference(Reference reference)
		{
			var existingReference = References.Where(k => k.ID == reference.ID).SingleOrDefault();
			if (existingReference != null)
			{
				reference.UpdatedAt = DateTime.Now;
				References.Remove(existingReference);
				References.Add(reference);
				return reference;
			}
			return null;
		}

		public void DeleteReference(Reference reference)
		{
			var existingReference = References.Where(k => k.ID == reference.ID).SingleOrDefault();
			if (existingReference != null)
				References.Remove(existingReference);
		}

		#endregion

		#region ProjectKeyword

		public void RemoveProjectKeywords()
		{
			var projectKeywords = ProjectKeywords.ToList();
			foreach (var element in projectKeywords)
			{
				ProjectKeywords.Remove(element);
			}
		}

		public virtual List<ProjectKeyword> GetProjectKeywords()
		{
			return ProjectKeywords.ToList();
		}

		public virtual ProjectKeyword GetProjectKeywordByID(int keywordID)
		{
			return ProjectKeywords.Where(k => k.KeywordID == keywordID).SingleOrDefault();
		}

		public virtual ProjectKeyword AddProjectKeyword(ProjectKeyword projectKeyword)
		{
			var existingKeyword = ProjectKeywords.Where(k => k.KeywordID == projectKeyword.KeywordID);
			if (existingKeyword.Any())
				return existingKeyword.FirstOrDefault();
			ProjectKeywords.Add(projectKeyword);
			return projectKeyword;
		}

		public virtual void DeleteProjectKeyword(ProjectKeyword projectKeyword)
		{
			var existingKeyword = GetProjectKeywordByID(projectKeyword.KeywordID);

			if (existingKeyword != null)
				ProjectKeywords.Remove(existingKeyword);
		}

		#endregion

		#region Skill

		public List<Skill> GetSkills()
		{
			return Skills.ToList();
		}

		public Skill AddSkill(Skill skill)
		{
			var existingSkill = Skills.Where(k => k.ID == skill.ID);
			if (existingSkill.Any())
				return existingSkill.FirstOrDefault();
			Skills.Add(skill);
			return skill;
		}

		public Skill UpdateSkill(Skill skill)
		{
			var existingSkill = Skills.Where(k => k.ID == skill.ID).SingleOrDefault();
			if (existingSkill != null)
			{
				skill.UpdatedAt = DateTime.Now;
				Skills.Remove(existingSkill);
				Skills.Add(skill);
				return skill;
			}
			return null;
		}

		public void DeleteSkill(Skill skill)
		{
			var existingSkill = Skills.Where(k => k.ID == skill.ID).SingleOrDefault();
			if (existingSkill != null)
				Skills.Remove(existingSkill);
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

	}
}