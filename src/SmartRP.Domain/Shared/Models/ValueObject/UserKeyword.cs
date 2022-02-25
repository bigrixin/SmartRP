namespace SmartRP.Domain
{
	public class UserKeyword
	{
		#region Properties

		public int ID { get; set; }

		public int UserID { get; set; }
		public int KeywordID { get; set; }

		#endregion

		#region Ctor

		protected UserKeyword()
		{
			// required by EF

		}

		public UserKeyword(int userID, int keywordID)
		{
			UserID = userID;
			KeywordID = keywordID;
		}

		#endregion
	}
}
