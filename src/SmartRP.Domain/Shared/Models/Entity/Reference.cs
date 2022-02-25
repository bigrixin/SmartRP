using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRP.Domain
{
	public class Reference
	{
		#region Properties

		public int ID { get; set; }
		public string Title { get; set; }
		public string Authors { get; set; }
		public string Citation { get; set; }
		public string ReferenceURL { get; set; }

		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }

		public int ProjectID { get; private set; }

		#endregion

		#region Ctor

		protected Reference()
		{
			// required by EF
		}

		public Reference(int projectID)
		{
			this.ProjectID = projectID;
			this.CreatedAt = DateTime.Now;
			this.UpdatedAt = DateTime.Now;
		}
		#endregion
	}
}
