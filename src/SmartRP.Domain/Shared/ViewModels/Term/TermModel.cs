using System;
using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class TermModel
	{
		[Key]
		public int ID { get; set; }

		[StringLength(30, ErrorMessage = "The {0} must be at long between {2} and {1}.", MinimumLength = 6)]
		[Display(Name = "Term Name")]
		public string TermName { get; set; }

		public int Year { get; set; }
		public Session Session { get; set; }

		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		//[DataType(DataType.Date)]
		[Display(Name = "Start Date")]
		public DateTime? StartAt { get; set; }

		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		//[DataType(DataType.Date)]
		[Display(Name = "End Date")]
		public DateTime? EndAt { get; set; }
		public int CoordinatorID { get; set; }
	}
}