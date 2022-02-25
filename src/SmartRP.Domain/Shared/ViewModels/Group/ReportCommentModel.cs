using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class ReportCommentViewModel : GroupViewModel
	{
		public int ReportID { get; set; }

		[StringLength(200, ErrorMessage = "The {0} must be at long between {2} and {1}.", MinimumLength = 6)]
		public string Comments { get; set; }
		public Grade? Grade { get; set; }
		[Display(Name = "Grade Comments")]
		public string GrageComments { get; set; }
	}
}
