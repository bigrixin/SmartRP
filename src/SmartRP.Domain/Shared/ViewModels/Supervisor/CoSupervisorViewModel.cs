using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class CoSupervisorViewModel: ProfileViewModel
	{
		[Display(Name = "My Supervisor")]
		public int MySupervisorID { get; set; }
		[Display(Name = "Supervisor")]
		public string MySupervisorName { get; set; }
		[Display(Name = "Enrolled Subjects")]
		public List<TermSubjectModel> EnrolledSubjects { get; set; }
	}
}