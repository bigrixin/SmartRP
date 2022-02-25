namespace SmartRP.Domain
{
	public class UserSkill
	{
		#region Properties

		public int ID { get; set; }

		public int UserID { get; set; }
		public int SkillID { get; set; }

		#endregion

		#region Ctor

		protected UserSkill()
		{
			// required by EF

		}

		public UserSkill(int userID, int skillID)
		{
			UserID = userID;
			SkillID = skillID;
		}

		#endregion
	}
}
