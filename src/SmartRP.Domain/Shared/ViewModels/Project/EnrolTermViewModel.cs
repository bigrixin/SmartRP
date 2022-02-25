using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SmartRP.Domain
{
	public class EnrolTermViewModel
	{
		[Key]
		public int ID { get; set; }

		public int TermId { get; set; }
		public int UserId { get; set; }
		public string UserType { get; set; }
		[Display(Name = "Semester Name")]
		public string TermName { get; set; }

		[Display(Name = "Start Date")]
		public DateTime? StartAt { get; set; }

		[Display(Name = "End Date")]
		public DateTime? EndAt { get; set; }
		public SelectList TermDropDownList { get; set; }
	}
}