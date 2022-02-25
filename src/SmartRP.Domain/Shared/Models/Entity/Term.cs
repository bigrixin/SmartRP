using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRP.Domain
{
	public class Term
	{
		#region Properties

		public int ID { get; set; }
		public string TermName { get; set; }
		public Session Session { get; set; }
		public DateTime? StartAt { get; set; }
		public DateTime? EndAt { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }

		public int CoordinatorID { get; private set; }
		public virtual ICollection<Subject> Subjects { get; set; }

		#endregion

		#region Ctor

		protected Term()
		{
			// required by EF
			Subjects = new List<Subject>();
		}

		public Term(int coordinatorID)
		{
			CoordinatorID = coordinatorID;
			Subjects = new List<Subject>();
			CreatedAt = DateTime.Now;
			UpdatedAt = DateTime.Now;
		}

		#endregion

		#region Subject

		public List<Subject> GetSubjects()
		{
			return Subjects.ToList();
		}

		public Subject GetSubject(int subjectID)
		{
			return Subjects.ToList().Where(s => s.ID == subjectID).SingleOrDefault();
		}

		public int GetSubjectIDByName(SubjectName subjectName)
		{
			int subjectID = 0;
			Subject subject = Subjects.ToList().Where(s => s.SubjectName == subjectName).SingleOrDefault();
			if (subject != null)
				subjectID = subject.ID;
			return subjectID;
		}

		public Subject AddSubject(Subject subject)
		{
			var existingSubject = Subjects.Where(k => k.SubjectName == subject.SubjectName).SingleOrDefault();
			if (existingSubject != null)
				return existingSubject;
			Subjects.Add(subject);
			return subject;
		}

		public Subject UpdateSubject(Subject subject)
		{
			var existingSubject = Subjects.Where(k => k.ID == subject.ID).SingleOrDefault();
			if (existingSubject != null)
			{
				Subjects.Remove(existingSubject);
				Subjects.Add(subject);
				return subject;
			}
			return null;
		}

		public void DeleteSubject(Subject subject)
		{
			var existingSubject = Subjects.Where(k => k.ID == subject.ID).SingleOrDefault();
			if (existingSubject != null)
				Subjects.Remove(existingSubject);
		}

		#endregion


	}
}