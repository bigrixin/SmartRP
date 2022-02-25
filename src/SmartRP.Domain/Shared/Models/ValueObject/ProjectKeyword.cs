namespace SmartRP.Domain
{
	public class ProjectKeyword
	{
		#region Properties

		public int ID { get; set; }

		public int ProjectID { get; set; }
		public int KeywordID { get; set; }

		#endregion

		#region Ctor

		protected ProjectKeyword()
		{
			// required by EF

		}

		public ProjectKeyword(int projectID, int keywordID)
		{
			ProjectID = projectID;
			KeywordID = keywordID;
		}

		#endregion
	}
}
