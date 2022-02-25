
using System.Collections.Generic;

namespace SmartRP.Domain
{
	public class CurrentTermStudentViewModel : StudentViewModel
	{
		public RequestStatus Status { get; set; }
		public List<Group> JoinedGroups { get; set; }
		public List<JoinProjectGroup> RequestedProjectGroups { get; set; }
	}
}