using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class MatchKeywordViewModel
	{
	  public int ID { get; set; }
		[Display(Name = "Keyword")]
		public string Title { get; set; }
		public string Description { get; set; }
		[Display(Name = "User Type")]
		public string UserType { get; set; }
		public int UserID { get; set; }
		public int KeywordID { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Degree { get; set; }
		public int MatchedKeywordCounter { get; set; }
	}
}