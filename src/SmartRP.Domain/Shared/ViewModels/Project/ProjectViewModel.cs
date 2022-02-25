using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class ProjectViewModel : EditKeywordsModel
	{
		[Key]
		public int ID { get; set; }

		[Display(Name = "Project Title")]
		public string Title { get; set; }

		[Display(Name = "Description / Abstract")]
		public string Description { get; set; }

		[Display(Name = "Status")]
		public ProjectStatus Status { get; set; }

		[Display(Name = "Semester")]
		public string SemesterName { get; set; }

		[Display(Name = "Subject")]
		public SubjectName SubjectName { get; set; }

		[Display(Name = "Skills Request")]
		public string SkillsRequest { get; set; }
		[Display(Name = "Group Size")]
		public int GroupSize { get; set; }
		[Display(Name = "Max Group No.")]
		public int MaxGroupNumber { get; set; }
		public string DocumentURL { get; set; }
		public String PublisherType { get; set; }
		[Display(Name = "Proposer")]
		public int UserID { get; set; }
 
		public int GroupID { get; set; }
		[Display(Name = "Group Name")]
		public string GroupName { get; set; }
		[Display(Name = "Group Description")]
		public string GroupDescription { get; set; }

		[Display(Name = "Expiry")]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime? ExpiredAt { get; set; }
		[Display(Name = "Create")]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime? CreatedAt { get; set; }

		[Display(Name = "Members")]
		public List<Student> GroupMembersList { get; set; }

		[Display(Name = "Project Groups")]
		public List<Group> ProjectGroups { get; set; }

		public List<Report> PostedReports { get; set; }
	}
}