using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class TermSubjectModel
	{
		[Display(Name = "Term Name")]
		public string TermName { get; set; }

		[Display(Name = "Subject Name")]
		public SubjectName SubjectName { get; set; }
	}
}