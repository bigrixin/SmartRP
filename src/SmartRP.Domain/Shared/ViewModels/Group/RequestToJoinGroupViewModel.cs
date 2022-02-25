using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class RequestToJoinGroupViewModel
	{
		public int ID { set; get; }
		public int StudentID { get; set; }
		public int ProjectID { get; set; }
		[Display(Name = "Project Title")]
		public string ProjectTitle { get; set; }
		[Display(Name = "Semester")]
		public string SemesterName { get; set; }
		[Display(Name = "Subject")]
		public SubjectName SubjectName { get; set; }
		public int GroupID { get; set; }
		[Display(Name = "Group Name")]
		public string GroupName { get; set; }
		[Display(Name = "Description")]
		public string GroupDescription { get; set; }
		public int ProposerID { get; set; }
		[Display(Name = "Approved Number")]
		public string ApprovedNumber { get; set; }
		public DateTime? UpdatedAt { get; set; }

		public RequestStatus RequestStatus { get; set; }
		//public string FontColor { get; set; }
		public virtual ICollection<Student> JoinedStudents { get; set; }
	}
}
