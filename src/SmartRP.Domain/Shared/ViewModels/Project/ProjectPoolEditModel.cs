using System;
using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class ProjectPoolEditModel : EditKeywordsModel
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

		[Display(Name = "Group Size")]
		public int GroupSize { get; set; }

		public int SupervisorID { get; set; }
		[Display(Name = "Created")]
		public DateTime? CreatedAt { get; set; }
		[Display(Name = "Updated")]
		public DateTime? UpdatedAt { get; set; }
	}
}