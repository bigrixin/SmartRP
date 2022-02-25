namespace SmartRP.Domain
{
	public class ProjectSkill
	{
		#region Properties

		public int ID { get; set; }

		public int ProjectID { get; set; }
		public int SkillID { get; set; }

		#endregion

		#region Ctor

		protected ProjectSkill()
		{
			// required by EF

		}

		public ProjectSkill(int projectID, int skillID)
		{
			ProjectID = projectID;
			SkillID = skillID;
		}

		#endregion
	}
}
