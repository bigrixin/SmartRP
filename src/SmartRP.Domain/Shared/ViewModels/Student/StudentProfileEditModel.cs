using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class StudentProfileEditModel: ProfileEditModel
	{
		//student
		[Required]
		[StringLength(8, ErrorMessage = "The {0} must be between {1} and {2} characters long.", MinimumLength = 6)]
		[RegularExpression("^[0-9]*$", ErrorMessage = "Student ID must be numeric")]
		[Display(Name = "Student ID")]
		public string StudentID { get; set; }
		[StringLength(50, ErrorMessage = "The {0} must be between {1} and {2} characters long.", MinimumLength = 3)]
		[Required]
		public string Degree { get; set; }
		[Range(2, 50, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
		public float? GPA { get; set; }
	}
}