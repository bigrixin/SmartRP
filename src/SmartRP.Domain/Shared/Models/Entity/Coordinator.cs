using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRP.Domain
{
	public class Coordinator : User
	{
		#region Properties

		public virtual ICollection<Term> Terms { get; set; }
		public virtual ICollection<Keyword> Keywords { get; set; }
		public virtual ICollection<Skill> Skills { get; set; }

		#endregion

		#region Ctor

		protected Coordinator()
		{
			// required by EF
			Terms = new List<Term>();
			Keywords = new List<Keyword>();
			Skills = new List<Skill>();
		}

		public Coordinator(string aspNetIdentity) : base(aspNetIdentity)
		{
			CreatedAt = DateTime.Now;
			UpdatedAt = DateTime.Now;
			Terms = new List<Term>();
			Keywords = new List<Keyword>();
			Skills = new List<Skill>();
		}

		#endregion

		#region action-term

		public Term GetTerm(int id)
		{
			return Terms.Where(t => t.ID == id).SingleOrDefault();
		}

		public Term AddTerm(Term term)
		{
			var existingTerm = Terms.Where(t => t.TermName == term.TermName);
			if (existingTerm.Any())
				return null;
			term.UpdatedAt = DateTime.Now;
			Terms.Add(term);
			return term;
		}

		public void DeleteTerm(Term term)
		{
			var existingTerm = Terms.Where(t => t.ID == term.ID).SingleOrDefault();
			if (existingTerm != null)
			{
				Terms.Remove(existingTerm);
			}
		}

		public void UpdateTerm(Term term)
		{
			var existingTerm = Terms.Where(t => t.ID == term.ID).SingleOrDefault();
			if (existingTerm != null)
			{
				term.UpdatedAt = DateTime.Now;
				Terms.Remove(existingTerm);
				Terms.Add(term);
			}
		}

		#endregion

		#region Keyword

		public virtual List<Keyword> GetKeywords()
		{
			return Keywords.ToList();
		}

		public virtual Keyword GetKeywordByID(int ID)
		{
			return Keywords.Where(k => k.ID == ID).SingleOrDefault();
		}

		public virtual Keyword AddKeyword(Keyword keyword)
		{
			var existingKeyword = Keywords.Where(k => k.ID == keyword.ID);
			if (existingKeyword.Any())
				return existingKeyword.FirstOrDefault();
			Keywords.Add(keyword);
			return keyword;
		}

		public virtual Keyword UpdateKeyword(Keyword keyword)
		{
			var existingKeyword = Keywords.Where(k => k.ID == keyword.ID).SingleOrDefault();
			if (existingKeyword != null)
			{
				Keywords.Remove(existingKeyword);
				Keywords.Add(keyword);
				return keyword;
			}
			return null;
		}

		public virtual void DeleteKeyword(Keyword keyword)
		{
			var existingKeyword = Keywords.Where(k => k.ID == keyword.ID).SingleOrDefault();
			if (existingKeyword != null)
				Keywords.Remove(existingKeyword);
		}

		#endregion

		#region Skill

		public virtual List<Skill> GetSkills()
		{
			return Skills.ToList();
		}

		public virtual Skill GetSkillByID(int ID)
		{
			return Skills.Where(k => k.ID == ID).SingleOrDefault();
		}

		public virtual Skill AddSkill(Skill skill)
		{
			var existingSkill = Skills.Where(k => k.ID == skill.ID);
			if (existingSkill.Any())
				return existingSkill.FirstOrDefault();
			Skills.Add(skill);
			return skill;
		}

		public virtual Skill UpdateSkill(Skill skill)
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

		public virtual void DeleteSkill(Skill skill)
		{
			var existingSkill = Skills.Where(k => k.ID == skill.ID).SingleOrDefault();
			if (existingSkill != null)
				Skills.Remove(existingSkill);
		}

		#endregion
	}
}