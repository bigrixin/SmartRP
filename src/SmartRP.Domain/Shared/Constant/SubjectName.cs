using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public enum SubjectName : int
	{
		[Display(Name = "TRP-32144 (6cp)")]
		TRP_32144_6cp,
		[Display(Name = "RP-32933 (6cp)")]
		RP_32933_6cp,
		[Display(Name = "RP-32934 (12cp)")]
		RP_32934_12cp
	}
}
