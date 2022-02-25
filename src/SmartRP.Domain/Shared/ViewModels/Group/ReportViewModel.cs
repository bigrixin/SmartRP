using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class ReportViewModel
	{
		[Key]
		public int ID { get; set; }
		[Display(Name = "Report Type")]
		public ReportType ReportType { get; set; }

		[Required]
		[StringLength(200, ErrorMessage = "The {0} must be at long between {2} and {1}.", MinimumLength = 6)]
		public string Description { get; set; }
		[Display(Name = "Upload File")]
		public string FileURL { get; set; }

		[StringLength(200, ErrorMessage = "The {0} must be at long between {2} and {1}.", MinimumLength = 6)]
		public string Comments { get; set; }
		public Grade? Grade { get; set; }

		public int GroupID { get; set; }  //many to one
	}
}