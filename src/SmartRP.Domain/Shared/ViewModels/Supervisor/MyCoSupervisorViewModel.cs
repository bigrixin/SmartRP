using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class MyCoSupervisorViewModel
	{
		public int ID { get; set; }
		[Display(Name = "Co-Supervisor Name")]
		public string Name { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		[Display(Name = "Projects in pool")]
		public int ProjectPoolCounter { get; set; }
		[Display(Name = "Published projects")]
		public int PublishedProjectCounter { get; set; }
	}
}