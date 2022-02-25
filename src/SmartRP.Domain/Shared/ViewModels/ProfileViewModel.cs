using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class ProfileViewModel
	{
		//user
		[Key]
		public int ID { get; set; }
		public string Firstname { get; set; }
		public string Lastname { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		[Display(Name = "About Myself")]
		public string Introduction { get; set; }
		[Display(Name = "Resume")]
		public string ResumeURL { get; set; }
		[Display(Name = "I suggested keywords")]
		public string SuggestedKeyword { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }

		//multi selects
		[Display(Name = "Research Keywords")]
		public List<Keyword> UserSelectedKeywords { get; set; }
		public List<Project> PostedProjects { get; set; }
		public string UserType { get; set; }
		public string UserID { get; set; }
	}
}