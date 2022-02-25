using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRP.Domain
{
	public class CoSupervisor : Supervisor
	{

		public int MySupervisorID { get; set; }
		public virtual ICollection<Subject> Subjects { get; set; }

		#region Ctor

		protected CoSupervisor()
		{
			// required by EF
			Subjects = new List<Subject>();
		}

		public CoSupervisor(string aspNetIdentity) : base(aspNetIdentity)
		{
			CreatedAt = DateTime.Now;
			UpdatedAt = DateTime.Now;
			Subjects = new List<Subject>();
		}

		#endregion

		#region Supervisor

		public void SetMySupervisor(int supervisorID)
		{
			MySupervisorID = supervisorID;
		}

		#endregion

		#region Subject

		public List<Subject> GetEnrolledSubjects()
		{
			return Subjects.ToList();
		}

		public List<Subject> GetCurrentTermEnrolledSubjects(int termID)
		{
			return Subjects.Where(s => s.TermID == termID).ToList();
		}

		public Subject FindEnrolledSubjectInTerm(int termID)
		{
			//return Subjects.Where(s => s.TermID == termID).SingleOrDefault();
			return Subjects.Where(s => s.TermID == termID).FirstOrDefault();
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
			return subject;
		}

		public void WithdrawSubject(Subject subject)
		{
			var existingSubject = Subjects.Where(k => k.ID == subject.ID).SingleOrDefault();
			if (existingSubject != null)
				Subjects.Remove(existingSubject);
		}

		#endregion

	}
}