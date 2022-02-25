using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SmartRP.Domain
{
	public class ExternalSupervisorProfileEditModel: ProfileEditModel
	{
		[Display(Name = "My Supervisor")]
		public int MySupervisorID { get; set; }
		[Display(Name = "Full Name")]
		public string MySupervisorName { get; set; }

		public IEnumerable<SelectListItem> SupervisorDropDownList { get; set; }
	}
}