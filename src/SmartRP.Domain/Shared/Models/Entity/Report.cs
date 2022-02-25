using System;

namespace SmartRP.Domain
{
	public class Report
	{
		#region Properties

		public int ID { get; set; }
		public ReportType ReportType { get; set; }
		public string Description { get; set; }
		public string FileURL { get; set; }
		public string Comments { get; set; }
		public Grade? Grade { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }

		public int GroupID { get; private set; }  //many to one

		#endregion

		protected Report()
		{
			// required by EF
		}


		#region Ctor

		public Report(int groupID)
		{
			this.GroupID = groupID;
			this.CreatedAt = DateTime.Now;
			this.UpdatedAt = DateTime.Now;
		}


		#endregion
	}
}