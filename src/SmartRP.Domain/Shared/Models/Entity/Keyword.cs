using System;

namespace SmartRP.Domain
{
	public class Keyword
	{

		#region Properties

		public int ID { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }

		public int CoordinatorID { get; set; }

		#endregion

		#region Ctor

		protected Keyword()
		{
			// required by EF
		}

		public Keyword(string title, string descripton)
		{
			this.Title = title;
			this.Description = descripton;
			this.CreatedAt = DateTime.Now;
			this.UpdatedAt = DateTime.Now;
		}

		#endregion


	}
}