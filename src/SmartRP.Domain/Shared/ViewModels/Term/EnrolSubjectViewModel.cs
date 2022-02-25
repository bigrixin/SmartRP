using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SmartRP.Domain
{
	public class EnrolSubjectViewModel
	{
		public int ID { get; set; }

		[Display(Name = "Semester Name")]
		public string TermName { get; set; }

		[Display(Name = "Start Date")]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime? StartAt { get; set; }

		[Display(Name = "End Date")]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime? EndAt { get; set; }

		[Display(Name = "Subject")]
		public SelectList SubjectNameDropDownList { get; set; }

		[Display(Name = "Subject")]
		public int SubjectID { get; set; }
		public string UserID { get; set; }
	}
}