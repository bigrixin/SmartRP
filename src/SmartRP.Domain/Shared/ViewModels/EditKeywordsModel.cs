using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class EditKeywordsModel
	{
		//multi selects
		[Display(Name = "Keywords")]
		public List<Keyword> KeywordList { get; set; }
		//tracking changes
		[Display(Name = "Research Keywords")]
		public List<Keyword> SelectedKeywords { get; set; }

		public int[] SelectedKeywordIDs { get; set; }
	}
}