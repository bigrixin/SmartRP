using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRP.Domain
{
	public class Subject
	{
		#region Properties

		public int ID { get; set; }
		public SubjectName SubjectName { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }

		public int TermID { get; private set; }
		public virtual ICollection<Student> Students { get; set; }
		public virtual ICollection<Project> Projects { get; set; }
		public virtual ICollection<CoSupervisor> CoSupervisors { get; set; }
		public virtual ICollection<ExternalSupervisor> ExternalSupervisors { get; set; }

		#endregion

		#region Ctor

		protected Subject()
		{
			// required by EF
			Students = new List<Student>();
			Projects = new List<Project>();
			CoSupervisors = new List<CoSupervisor>();
			ExternalSupervisors = new List<ExternalSupervisor>();
		}

		public Subject(int termID)
		{
			TermID = termID;
			CreatedAt = DateTime.Now;
			UpdatedAt = DateTime.Now;
		}

		#endregion

		#region Student

		public Student GetStudent(int studentID)
		{
			return Students.Where(a => a.ID == studentID).SingleOrDefault();
		}

		public List<Student> GetStudents()
		{
			return Students.ToList();
		}

		public Student AddStudent(Student student)
		{
			var existingStudent = Students.Where(s => s.ID == student.ID);
			if (existingStudent.Any())
				return existingStudent.FirstOrDefault();
			student.CreatedAt = DateTime.Now;
			student.UpdatedAt = DateTime.Now;
			Students.Add(student);
			return student;
		}

		public Student UpdateStudent(Student student)
		{
			var existingStudent = Students.Where(s => s.ID == student.ID).SingleOrDefault();
			if (existingStudent != null)
			{
				student.UpdatedAt = DateTime.Now;
				Students.Remove(existingStudent);
				Students.Add(student);
				return student;
			}
			return null;
		}

		public void DeleteStudent(Student student)
		{
			var existingStudent = Students.Where(s => s.ID == student.ID).SingleOrDefault();
			if (existingStudent != null)
				Students.Remove(student);
		}

		#endregion

		#region CoSupervisor

		public CoSupervisor GetCoSupervisor(int coSupervisorID)
		{
			return CoSupervisors.Where(a => a.ID == coSupervisorID).SingleOrDefault();
		}

		public List<CoSupervisor> GetCoSupervisors()
		{
			return CoSupervisors.ToList();
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
