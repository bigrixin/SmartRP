using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRP.Domain
{
	public class Project
	{
		#region Properties

		public int ID { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string DocumentURL { get; set; }
		public string SkillsRequest { get; set; }
		public ProjectStatus Status { get; set; }
		public string ApprovedNumber { get; set; }
		public int GroupSize { get; set; }
		public int MaxGroupNumber { get; set; }
		public DateTime? ExpiredAt { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }

		public String PublisherType { get; set; }

		public int UserID { get; set; }
		public int SubjectID { get; set; }
		public int? ProjectPoolID { get; set; }
		public virtual ICollection<Group> Groups { get; set; }
		public virtual ICollection<Reference> References { get; set; }
		public virtual ICollection<ProjectKeyword> ProjectKeywords { get; set; }
		public virtual ICollection<Skill> Skills { get; set; }
		public virtual ICollection<TRPToRP> TRPToRPs { get; set; }

		#endregion

		#region Ctor

		protected Project()
		{
			// required by EF
			Groups = new List<Group>();
			References = new List<Reference>();
			ProjectKeywords = new List<ProjectKeyword>();
			Skills = new List<Skill>();
			TRPToRPs = new List<TRPToRP>();
		}

		public Project(int userID)
		{
			UserID = userID;
			CreatedAt = DateTime.Now;
			UpdatedAt = DateTime.Now;
		  Status = ProjectStatus.Pending;    ///???????
			Groups = new List<Group>();
			References = new List<Reference>();
		  ProjectKeywords = new List<ProjectKeyword>();
			Skills = new List<Skill>();
			TRPToRPs = new List<TRPToRP>();
		}

		public Project(int userID, string userType)
		{
			UserID = userID;
			PublisherType = userType;
		  Status = ProjectStatus.Opening;
			CreatedAt = DateTime.Now;
			UpdatedAt = DateTime.Now;
			Groups = new List<Group>();
			References = new List<Reference>();
			ProjectKeywords = new List<ProjectKeyword>();
			Skills = new List<Skill>();
			TRPToRPs = new List<TRPToRP>();
		}

		#endregion

		#region Group

		public List<Group> GetGroups()
		{
			return Groups.ToList();
		}

		public Group GetGroup(int groupID)
		{
			return Groups.Where(g => g.ID == groupID).SingleOrDefault();
		}

		public void RemoveGroups()
		{
			var groups = Groups.ToList();
			foreach (var element in groups)
			{
				Groups.Remove(element);
			}
		}

		public Group AddGroup(Group group)
		{
			var existingGroup = Groups.Where(k => k.ID == group.ID);
			if (existingGroup.Any())
				return existingGroup.FirstOrDefault();
			Groups.Add(group);
			return group;
		}

		public Group UpdateGroup(Group group)
		{
			var existingGroup = Groups.Where(k => k.ID == group.ID).SingleOrDefault();
			if (existingGroup != null)
			{
				group.UpdatedAt = DateTime.Now;
				Groups.Remove(existingGroup);
				Groups.Add(group);
				return group;
			}
			return null;
		}

		public void DeleteGroup(Group group)
		{
			var existingGroup = Groups.Where(k => k.ID == group.ID).SingleOrDefault();
			if (existingGroup != null)
				Groups.Remove(existingGroup);
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

		#region TRPToRP

		public List<TRPToRP> GetTRPToRPs()
		{
			return TRPToRPs.ToList();
		}

		public TRPToRP AddTRPToRP(TRPToRP tRPToRP)
		{
			var existingTRPToRP = TRPToRPs.Where(k => k.ID == tRPToRP.ID);
			if (existingTRPToRP.Any())
				return existingTRPToRP.FirstOrDefault();
			TRPToRPs.Add(tRPToRP);
			return tRPToRP;
		}

		public TRPToRP UpdateTRPToRP(TRPToRP tRPToRP)
		{
			var existingTRPToRP = TRPToRPs.Where(k => k.ID == tRPToRP.ID).SingleOrDefault();
			if (existingTRPToRP != null)
			{
				tRPToRP.UpdatedAt = DateTime.Now;
				TRPToRPs.Remove(existingTRPToRP);
				TRPToRPs.Add(tRPToRP);
				return tRPToRP;
			}
			return null;
		}

		public void DeleteTRPToRP(TRPToRP tRPToRP)
		{
			var existingTRPToRP = TRPToRPs.Where(k => k.ID == tRPToRP.ID).SingleOrDefault();
			if (existingTRPToRP != null)
				TRPToRPs.Remove(existingTRPToRP);
		}

		#endregion
	}
}