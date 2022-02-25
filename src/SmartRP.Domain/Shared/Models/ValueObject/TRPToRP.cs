using System;

namespace SmartRP.Domain
{
	public class TRPToRP
	{
		#region Properties

		public int ID { get; set; }

		public int NewProjectID { get; set; }
		public string Comments { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }

		public int ProjectID { get; set; }

		#endregion

		#region Ctor

		protected TRPToRP()
		{
			// required by EF

		}

		#endregion
	}
}
