using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class TermEditModel: TermModel
	{
		[Display(Name = "Subject")]
		public List<SubjectNameModel> CheckBoxSubjectNames { get; set; }
	}
}