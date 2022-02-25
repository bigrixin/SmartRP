using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static SmartRP.Domain.Group;

namespace SmartRP.Domain
{
	public class GroupViewModel
	{
		public int ID { get; set; }
		[Required]
		[Display(Name = "Group Name")]
		[StringLength(20, ErrorMessage = "The {0} should have {2} - {1} long (with space)", MinimumLength = 3)]
		public string Name { get; set; }
		[Required]
		[Display(Name = "Group Description")]
		[StringLength(100, ErrorMessage = "The {0} should have {2} - {1} long (with space)", MinimumLength = 3)]
		public string Description { get; set; }
		[Display(Name = "Approved Number")]
		public string ApprovedNO { get; set; }
		public GroupStatus Status { get; set; }
		public int Size { get; set; }
		public int Vacancy { get; set; }
		[Display(Name = "Project ID")]
		public int ProjectID { get; set; }
		public int LeaderID { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public int? SupervisorID { get; set; }
		public int? CoSupervisorID { get; set; }
		public int? ExtSupervisorID { get; set; }
		public int PublisherID { get; set; }
		public virtual ICollection<Student> JoinedStudents { get; set; }
		[Display(Name = "Group Members")]
		public List<Student> GroupMembersList { get; set; }
		[Display(Name = "Reports")]
		public List<Report> PostedReports { get; set; }
	}
}
