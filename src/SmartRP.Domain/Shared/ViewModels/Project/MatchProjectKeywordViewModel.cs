using System.Collections.Generic;

namespace SmartRP.Domain
{
	public class MatchProjectKeywordViewModel
	{
		public int UserID { get; set; }
		public int MatchedKeywordCounter { get; set; }
		public List<Keyword> Keywords { get; set; }
		public string UserType { get; set; }
	}
}