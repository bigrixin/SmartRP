
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class StudentViewModel: ProfileViewModel
	{
		[Display(Name = "Student ID")]
		public string StudentID { get; set; }
		public string Degree { get; set; }
		public float? GPA { get; set; }

		[Display(Name = "Photo")]
		public string PhotoURL { get; set; }

		[Display(Name = "Joined a group")]
		public bool HasJoinedCurrentSubjectProjectGroup { get; set; }
		[Display(Name = "Enrolled Subjects")]
		public List<TermSubjectModel> EnrolledSubjects { get; set; }
		public int PreProjectID { get; set; }
	}
}