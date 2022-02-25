using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class KeywordViewModel
	{
		[Key]
		public int ID { get; set; }
		[Required(AllowEmptyStrings = false)]
		[StringLength(50, ErrorMessage = "The {0} must be at least {2} characters and less than 50 long.", MinimumLength = 3)]
		[Display(Name = "Keyword")]
		public string Title { get; set; }

		[StringLength(550, ErrorMessage = "The {0} must be less than {1} characters long.")]
		[Display(Name = "Description")]
		public string Description { get; set; }

		[Display(Name = "Coordinator")]
		public int CoordinatorID { get; set; }
	}
}