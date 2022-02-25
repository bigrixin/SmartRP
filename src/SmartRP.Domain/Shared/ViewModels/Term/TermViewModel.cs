using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class TermViewModel: TermModel
	{
		[Display(Name = "Subject")]
		public List<Subject> Subjects { get; set; }
	}
}