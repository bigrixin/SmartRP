using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SmartRP.Domain
{
	public class ProjectEditModel : EditKeywordsModel
	{
		[Key]
		public int ID { get; set; }

		[Required(AllowEmptyStrings = false)]
		[StringLength(150, ErrorMessage = "The {0} shoud have {2} - {1} character long (with space)", MinimumLength = 6)]
		[Display(Name = "Title")]
		public string Title { get; set; }

		[Required(AllowEmptyStrings = false)]
		[StringLength(2000, ErrorMessage = "The {0} should have {2} - {1} long (with space)", MinimumLength = 6)]
		[Display(Name = "Description / Abstract")]
		public string Description { get; set; }
		[Display(Name = "Skills Request")]
		public string SkillsRequest { get; set; }

		[Display(Name = "Attachment")]
		public string DocumentURL { get; set; }
		[Display(Name = "Project Status")]
		public ProjectStatus Status { get; set; }
		[Display(Name = "Subject")]
		public SubjectName SubjectName { get; set; }
		public int SubjectID { get; set; }

		[Range(1, 10, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
		[Display(Name = "Group Size")]
		public int GroupSize { get; set; }
		public int JoinedGroupMaxStudents { get; set; }

		[Display(Name = "Max Group No.")]
		[Range(1, 5, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
		public int MaxGroupNumber { get; set; }
		public String PublisherType { get; set; }
		[Display(Name = "Proposer")]
		public int UserID { get; set; }

		[Display(Name = "Group Name")]
		[StringLength(20, ErrorMessage = "The {0} should have {2} - {1} long (with space)", MinimumLength = 3)]
		public string GroupName { get; set; }

		[Display(Name = "Group Description")]
		[StringLength(100, ErrorMessage = "The {0} should have {2} - {1} long (with space)", MinimumLength = 3)]
		public string GroupDescription { get; set; }

		[Required]
		[Display(Name = "Expiry")]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime? ExpiredAt { get; set; }

		public int ProjectPoolID { get; set; }
		public int CurrentEnrolledSubjectID { get; set; }
		[Display(Name = "Select Subject")]
		public IEnumerable<SelectListItem> SubjectList { get; set; }
	}
}