using System.ComponentModel.DataAnnotations;

namespace SmartRP.Domain
{
	public class ProfileEditModel: EditKeywordsModel
	{
 
		[Key]
		public int ID { get; set; }
		[Required(AllowEmptyStrings = false)]
		[StringLength(30, ErrorMessage = "The {0} must be between {1} and {2} characters long.", MinimumLength = 2)]
		public string Firstname { get; set; }
		[Required(AllowEmptyStrings = false)]
		[StringLength(30)]
		public string Lastname { get; set; }

		[EmailAddress]
		[Required(AllowEmptyStrings = false)]
		public string Email { get; set; }
		[RegularExpression(@"^\s*\+?\s*([0-9][\s-]*){10}$", ErrorMessage = "Phone number must be numeric")]  //^[0-9 ]*$
		public string Phone { get; set; }
 
		[StringLength(300, ErrorMessage = "Maximun 300 characters long.")]
		[Display(Name = "About Myself")]
		public string Introduction { get; set; }
		[Display(Name = "Resume")]
		public string ResumeURL { get; set; }
		[Display(Name = "Input suggested keywords")]
		public string SuggestedKeyword { get; set; }
		public string UserType { get; set; }
		public string UserID { get; set; }
	}
}