using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class RequestsViewModel
	{
		[Display(Name = "Project ID")]
		public int ID { get; set; }

		[Display(Name = "Project Title")]
		public string Title { get; set; }

		[Display(Name = "Proposer")]
		public string ProposerName { get; set; }

		[Display(Name = "Proposer Email")]
		public string Email { get; set; }

		[Display(Name = "Group Size")]
		public int GroupSize { get; set; }
		[Display(Name = "Joined Students")]
		public int JoinedStudents { get; set; }

		public int StudentID { get; set; }

	}
}