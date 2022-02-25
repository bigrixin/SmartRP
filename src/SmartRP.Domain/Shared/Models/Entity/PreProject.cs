using System;
using System.Collections.Generic;

namespace SmartRP.Domain
{
	public class PreProject : Project
	{
		#region properties

		public PreProjectStatus PreProjectStatus { get; set; }

		public int SupervisorID { get; private set; }
		public virtual ICollection<Student> Students { get; set; }

		#endregion

		#region Ctor

		protected PreProject()
		{
			// required by EF
		}

		#endregion
	}
}
