using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRP.Domain
{
	public class Group
	{

		#region Properties

		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public GroupStatus Status { get; set; }
		public int Size { get; set; }
		public int Vacancy { get; set; }
		public int? LeaderID { get; set; }
		public string ApprovedNO { get; set; }
		public Grade? Grade { get; set; }
		public string Comments { get; set; }
		public DateTime? CommentDate { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }

		public int? SupervisorID { get; set; }
		public int? CoSupervisorID { get; set; }
		public int? ExtSupervisorID { get; set; }
		public int ProjectID { get; private set; }

		public virtual ICollection<Student> Students { get; set; }   //many to many
		public virtual ICollection<Report> Reports { get; set; }     //one to many

		#endregion

		#region Ctor

		protected Group()
		{
			// required by EF   
			Students = new List<Student>();
			Reports = new List<Report>();
		}

		//for student post project
		public Group(int projectID, int leaderID, string name, string description, int size)
		{
			ProjectID = projectID;
			LeaderID = leaderID;
			Name = name;
			Description = description;
			Size = size;
			Vacancy = size;
			Status = GroupStatus.Avaliable;
			CreatedAt = DateTime.Now;
			UpdatedAt = DateTime.Now;
			Students = new List<Student>();
			Reports = new List<Report>();
		}

		//for supervisor post project
		public Group(int projectID, string name, string description, int size)
		{
			ProjectID = projectID;
			Name = name;
			Description = description;
			Size = size;
			Vacancy = size;
			CreatedAt = DateTime.Now;
			UpdatedAt = DateTime.Now;
			Students = new List<Student>();
			Reports = new List<Report>();
		}

		#endregion

		#region User
		public void SetLeader(int studentID)
		{
			LeaderID = studentID;
		}

		public void SetSupervisor(int supervisorID)
		{
			SupervisorID = supervisorID;
		}

		public void SetCoSupervisor(int coSupervisorID)
		{
			CoSupervisorID = coSupervisorID;
		}

		public void SetExternalSupervisor(int extSupervisorID)
		{
			ExtSupervisorID = extSupervisorID;
		}

		#endregion

		#region Student

		public List<Student> GetStudents()
		{
			return Students.ToList();
		}

		public Student GetStudent(int studentID)
		{
			return Students.Where(s => s.ID == studentID).SingleOrDefault();
		}

		public void RemoveStudents()
		{
			var students = Students.ToList();
			foreach (var element in students)
			{
				Students.Remove(element);
			}
		}

		public Student AddStudent(Student Student)
		{
			var existingStudent = Students.Where(k => k.ID == Student.ID);
			if (existingStudent.Any())
				return existingStudent.FirstOrDefault();
			if (Students.Count() < Size)
			{
				Students.Add(Student);
				Vacancy = Vacancy - 1;
				if (Students.Count() == Size)
				{
					Status = GroupStatus.Full;
					Vacancy = 0;
				}

			}
			return Student;
		}

		public void DeleteStudent(Student Student)
		{
			var existingStudent = Students.Where(k => k.ID == Student.ID).SingleOrDefault();
			if (existingStudent != null)
			{
				Students.Remove(existingStudent);
				Status = GroupStatus.Avaliable;
				if (Students.Count() < Size)
					Vacancy = Vacancy + 1;
				if (Students.Count() == 0)
					LeaderID = null;
			}
		}

		#endregion

		#region Report

		public List<Report> GetReports()
		{
			return Reports.ToList();
		}

		public Report GetReport(int reportID)
		{
			return Reports.Where(r => r.ID == reportID).SingleOrDefault();
		}

		public Report AddReport(Report report)
		{
			var existingReport = Reports.Where(k => k.ID == report.ID);
			if (existingReport.Any())
				return existingReport.FirstOrDefault();
			Reports.Add(report);
			return report;
		}

		public Report UpdateReport(Report report)
		{
			var existingReport = Reports.Where(k => k.ID == report.ID).SingleOrDefault();
			if (existingReport != null)
			{
				Reports.Remove(existingReport);
				Reports.Add(report);
				return report;
			}
			return null;
		}

		public void DeleteReport(Report report)
		{
			var existingReport = Reports.Where(k => k.ID == report.ID).SingleOrDefault();
			if (existingReport != null)
				Reports.Remove(existingReport);
		}

		#endregion
	}
}