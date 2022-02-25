using RazorEngine;
using RazorEngine.Templating;
using SmartRP.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace SmartRP.Domain.Service
{
    public class CommonService : ICommonService
	{
		#region Fields

		private readonly IWriteEntities _writeEntities;
		private readonly IReadEntities _readEntities;

		#endregion

		#region Ctor

		public CommonService(IWriteEntities writeEntities, IReadEntities readEntities)
		{
			_writeEntities = writeEntities;
			_readEntities = readEntities;
		}

		#endregion

		#region Keyword 

		public List<Keyword> GetKeywordList()
		{
			var keywords = _readEntities.Get<Keyword>();
			return keywords.ToList();
		}

		public List<Keyword> GetUserSelectedKeywordsByUserID(int userID)
		{
			List<UserKeyword> userKeywords = new List<UserKeyword>();
			userKeywords = _readEntities.Get<User>(u => u.ID == userID).Single().GetUserKeywords();
			List<Keyword> keywords = new List<Keyword>();
			foreach (var item in userKeywords)
			{
				var keyword = _readEntities.Get<Keyword>(k => k.ID == item.KeywordID).SingleOrDefault();
				if (keyword != null)
					keywords.Add(keyword);
			}
			return keywords.OrderBy(a => a.ID).ToList();
		}

		public List<Keyword> GetUserSelectedKeywords(int[] selectedKeywordIDs)
		{
			List<Keyword> keywords = new List<Keyword>();
			foreach (var item in selectedKeywordIDs)
			{
				var keyword = _readEntities.Get<Keyword>(a => a.ID == item).SingleOrDefault();
				if (keyword != null)
					keywords.Add(keyword);
			}
			return keywords;
		}


		public List<Keyword> GetProjectSelectedKeywordsByProjectID(int projectID)
		{
			List<ProjectKeyword> projectKeywords = new List<ProjectKeyword>();
			List<Keyword> keywords = new List<Keyword>();
			var project = _readEntities.Get<Project>(u => u.ID == projectID).SingleOrDefault();
			if (project != null)
			{
				projectKeywords = project.GetProjectKeywords();
				foreach (var item in projectKeywords)
				{
					var keyword = _readEntities.Get<Keyword>(k => k.ID == item.KeywordID).SingleOrDefault();
					if (keyword != null)
						keywords.Add(keyword);
				}
			}
			return keywords.OrderBy(a => a.ID).ToList();
		}


		#endregion

		#region Profile

		public void UpdateProfile(User user, int[] selectedKeywordIDs)
		{

			if (user == null)
				throw new ArgumentNullException("user");

			if (selectedKeywordIDs != null)
			{
				//add new selected
				foreach (int selectId in selectedKeywordIDs)
				{
					var userKeyword = user.GetUserKeywordByID(selectId);
					if (userKeyword == null)
					{
						UserKeyword newUserKeyword = new UserKeyword(user.ID, selectId);
						user.AddUserKeyword(newUserKeyword);
					}
				}
			}

			_writeEntities.Update(user);
			_writeEntities.Save();

			var newUserKeywords = _readEntities.Get<UserKeyword>(a => a.UserID == user.ID);
			if (newUserKeywords != null)
			{
				//delete unselect items
				foreach (var element in newUserKeywords)
				{
					bool find = false;

					if (selectedKeywordIDs != null)
					{
						foreach (int selectId in selectedKeywordIDs)
						{
							if (element.KeywordID == selectId)
							{
								find = true;
								break;
							}
						}
					}
					if (!find)
					{
						_writeEntities.Delete(element);
						_writeEntities.Update(user);
						_writeEntities.Save();
					}
				}
			}
		}

		public List<SelectListItem> GetSupervisorSelectList()
		{
			var supervisors = _readEntities.Get<Supervisor>(a => a.SupervisorType == SupervisorType.Principal).ToList();
			var selectListItems = supervisors.Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Firstname + " " + x.Lastname }).ToList();
			selectListItems.Insert(0, new SelectListItem() { Value = "", Text = "" });
			return selectListItems;
		}

		#endregion

		#region Term

		public List<SubjectNameModel> GetInitialSubjectNames(int termID)
		{
			List<SubjectNameModel> subjectNames = new List<SubjectNameModel>();

			foreach (SubjectName item in Enum.GetValues(typeof(SubjectName)))
			{
				subjectNames.Add(new SubjectNameModel()
				{
					SubjectName = item,
					IsSelected = false
				});
			}

			var term = _readEntities.Get<Term>(a => a.ID == termID).SingleOrDefault();
			if (term != null)
			{
				foreach (var item in subjectNames)
				{
					foreach (var subject in term.Subjects)
					{
						if (subject.SubjectName == item.SubjectName)
							item.IsSelected = true;
					}
				}
			}

			return subjectNames;
		}

		public List<Subject> GetSubjectsFromModel(List<SubjectNameModel> subjectNames, int termID)
		{
			//add subject in a term
			List<Subject> subjects = new List<Subject>();
			DateTime now = DateTime.Now;
			foreach (var item in subjectNames)
			{
				if (item.IsSelected)
				{
					Subject suject = new Subject(termID)
					{
						CreatedAt = now,
						UpdatedAt = now,
						SubjectName = item.SubjectName
					};
					subjects.Add(suject);
				}
			}
			return subjects;

		}

		public DateTime GetTermStartTime(Session session, int year)
		{
			DateTime startAt = DateTime.Now;
			switch (session)
			{
				case Session.Autumn:
					startAt = DateTime.Parse(ConfigurationManager.AppSettings["AutumnStart"] + year);
					break;
				case Session.Spring:
					startAt = DateTime.Parse(ConfigurationManager.AppSettings["SpringStart"] + year);
					break;
				case Session.Summer:
					startAt = DateTime.Parse(ConfigurationManager.AppSettings["SummerStart"] + year);
					break;
			}
			return startAt;
		}

		public DateTime GetTermEndTime(Session session, int year)
		{
			DateTime endAt = DateTime.Now;
			switch (session)
			{
				case Session.Autumn:
					endAt = DateTime.Parse(ConfigurationManager.AppSettings["AutumnEnd"] + year);
					break;
				case Session.Spring:
					endAt = DateTime.Parse(ConfigurationManager.AppSettings["SpringEnd"] + year);
					break;
				case Session.Summer:
					endAt = DateTime.Parse(ConfigurationManager.AppSettings["SummerEnd"] + (Convert.ToInt32(year) + 1));
					break;
			}
			return endAt;
		}

		public List<Term> GetTermList()
		{
			var terms = _readEntities.Get<Term>();
			return terms.ToList();
		}

		public SelectList GetTermSelectList()
		{
			return new SelectList(GetTermList(), "Id", "TermName");
		}

		public SelectList GetTermSubjectSelectList(int termID)
		{
			DateTime now = DateTime.Today;
			var termSubjects = _readEntities.Get<Subject>().Where(a => a.TermID == termID);
			if (termSubjects == null)
				return null;

			return new SelectList(termSubjects.ToList(), "ID", "SubjectName");
		}

		public Term GetCurrentOpenTerm()
		{
			DateTime now = DateTime.Today;
			var currentTerm = _readEntities.Get<Term>().Where(a => a.StartAt <= now && now <= a.EndAt).SingleOrDefault();
			return currentTerm;
		}

		public List<Subject> GetEnrolledTermSubjects(int userID, string userType)
		{
			List<Subject> enrolledSubjects = new List<Subject>();
			switch (userType)
			{
				case "Student":
					var student = _readEntities.Get<Student>(a => a.ID == userID).SingleOrDefault();
					enrolledSubjects = student.GetEnrolledSubjects();
					break;
				case "CoSupervisor":
					var coSupervisor = _readEntities.Get<CoSupervisor>(a => a.ID == userID).SingleOrDefault();
					enrolledSubjects = coSupervisor.GetEnrolledSubjects();
					break;
				case "ExternalSupervisor":
					var externalSupervisor = _readEntities.Get<ExternalSupervisor>(a => a.ID == userID).SingleOrDefault();
					enrolledSubjects = externalSupervisor.GetEnrolledSubjects();
					break;
			}
			return enrolledSubjects;
		}


		public List<TermSubjectModel> GetEnrolledTermSubjectsVMList(int userID, string userType)
		{
			List<TermSubjectModel> termSubjects = new List<TermSubjectModel>();
			List<Subject> enrolledSubjects = GetEnrolledTermSubjects(userID, userType);

			foreach (var element in enrolledSubjects)
			{
				var term = _readEntities.Get<Term>(a => a.ID == element.TermID).SingleOrDefault();
				if (term != null)
				{
					TermSubjectModel termSubject = new TermSubjectModel();
					termSubject.SubjectName = element.SubjectName;
					termSubject.TermName = term.TermName;
					termSubjects.Add(termSubject);
				}
			}
			return termSubjects;
		}

		public Subject UserEnrolSubject(string userType, EnrolSubjectViewModel model)
		{
			var subject = _readEntities.Get<Subject>(s => s.ID == model.SubjectID).SingleOrDefault();
			if (subject == null)
				return null;
			switch (userType)
			{
				case "Student":
					var student = _readEntities.Get<Student>(a => a.LoginIdentityID == model.UserID).SingleOrDefault();
					if (student.FindEnrolledSubjectInTerm(subject.TermID) == null)
					{
						student.EnrolSubject(subject);
						_writeEntities.Update(student);
						_writeEntities.Save();
					}
					else
						subject = null;
					break;
				case "CoSupervisor":
					var coSupervisor = _readEntities.Get<CoSupervisor>(a => a.LoginIdentityID == model.UserID).SingleOrDefault();
					coSupervisor.EnrolSubject(subject);
					_writeEntities.Update(coSupervisor);
					_writeEntities.Save();
					break;
				case "ExternalSupervisor":
					var externalSupervisor = _readEntities.Get<ExternalSupervisor>(a => a.LoginIdentityID == model.UserID).SingleOrDefault();
					externalSupervisor.EnrolSubject(subject);
					_writeEntities.Update(externalSupervisor);
					_writeEntities.Save();
					break;
			}
			return subject;
		}

		public IEnumerable<CoSupervisor> GetCurrentSemesterCoSupervisors()
		{
			Term currentTerm = GetCurrentOpenTerm();
			if (currentTerm == null)
				return null;
			IEnumerable<CoSupervisor> coSupervisors = null;
			int i = 0;
			var subjects = currentTerm.GetSubjects();
			foreach (var subject in subjects)
			{
				if (i == 0)
					coSupervisors = subject.GetCoSupervisors();
				else
				{
					var nextCoSupervisors = subject.GetCoSupervisors();
					coSupervisors = coSupervisors.Concat(nextCoSupervisors);
				}

				i++;
			}
			return coSupervisors.Distinct();
		}


		public IEnumerable<Student> GetCurrentSemesterStudents()
		{
			Term currentTerm = GetCurrentOpenTerm();
			if (currentTerm == null)
				return null;
			IEnumerable<Student> students = null;
			int i = 0;
			foreach (var subject in currentTerm.GetSubjects())
			{
				if (i == 0)
					students = subject.GetStudents();
				else
				{
					var nextStudents = subject.GetStudents();
					students = students.Concat(nextStudents);
				}

				i++;
			}
			return students;
		}

		public IEnumerable<Student> GetCurrentSubjectStudents(int subjectID)
		{
			Subject subject = _readEntities.Get<Subject>(s => s.ID == subjectID).SingleOrDefault();
			if (subject == null)
				return null;
			return subject.GetStudents().OrderByDescending(a => a.ID);
		}

		#endregion

		#region Subject

		public SubjectName GetSubjectNameByID(int subjectID)
		{
			Subject subject = _readEntities.Get<Subject>(s => s.ID == subjectID).SingleOrDefault();
			if (subject != null)
				return subject.SubjectName;
			return 0;
		}

		public Term GetSemesterBySubjectID(int subjectID)
		{
			Subject subject = _readEntities.Get<Subject>(s => s.ID == subjectID).SingleOrDefault();
			if (subject != null)
			{
				Term term = _readEntities.Get<Term>(t => t.ID == subject.TermID).SingleOrDefault();
				if (term != null)
					return term;
			}
			return null;
		}

		#endregion

		#region Current subject users

		public IEnumerable<Student> GetEnrolledStudentsBySemester(Term currentTerm)
		{
			if (currentTerm == null)
				return null;
			IEnumerable<Student> students = null;
			int i = 0;
			foreach (var subject in currentTerm.GetSubjects())
			{
				if (i == 0)
					students = subject.GetStudents();
				else
				{
					var nextStudents = subject.GetStudents();
					if (nextStudents != null)
						students = students.Concat(nextStudents);
				}

				i++;
			}
			return students;
		}

		public Subject GetCurrentStudentEnrolledSubject(Student currentStudent)
		{
			if (currentStudent == null)
				return null;
			Term currentTerm = GetCurrentOpenTerm();
			if (currentTerm == null)
				return null;
			Subject subject = currentStudent.FindEnrolledSubjectInTerm(currentTerm.ID);
			return subject;
		}

		public Subject GetCurrentCoSupervisorEnrolledSubject(CoSupervisor currentCoSupervisor)
		{
			if (currentCoSupervisor == null)
				return null;
			Term currentTerm = GetCurrentOpenTerm();
			if (currentTerm == null)
				return null;
			Subject subject = currentCoSupervisor.FindEnrolledSubjectInTerm(currentTerm.ID);
			return subject;
		}

		public Subject GetCurrentExternalSupervisorEnrolledSubject(ExternalSupervisor currentExternalSupervisor)
		{
			if (currentExternalSupervisor == null)
				return null;
			Term currentTerm = GetCurrentOpenTerm();
			if (currentTerm == null)
				return null;
			Subject subject = currentExternalSupervisor.FindEnrolledSubjectInTerm(currentTerm.ID);
			return subject;
		}

		#endregion

		#region Send Email

		public void SendStudentRequestEmail(string subject, Project project, string fromUserName, string toUserName, string toEmailAddress)
		{
			var context = getEmailCallbackContext(project, fromUserName, toUserName);
			var body = RenderPartialViewToString("~/Views/Email/StudentRequestEmail.cshtml", context);
			SendEmail(subject, body, toEmailAddress);
		}
		public void SendAcceptedRequestEmail(string subject, Project project, string fromUserName, string toUserName, string toEmailAddress)
		{
			var context = getEmailCallbackContext(project, fromUserName, toUserName);
			var body = RenderPartialViewToString("~/Views/Email/AcceptRequestEmail.cshtml", context);
			SendEmail(subject, body, toEmailAddress);
		}

		public void SendRejectedRequestEmail(string subject, Project project, string fromUserName, string toUserName, string toEmailAddress)
		{
			var context = getEmailCallbackContext(project, fromUserName, toUserName);
			var body = RenderPartialViewToString("~/Views/Email/RejectRequestEmail.cshtml", context);
			SendEmail(subject, body, toEmailAddress);
		}

		public void SendEmailToProjectGroup(string subject, Project project, string fromUserName, string toUserName, string toEmailAddress)
		{
			var context = getEmailCallbackContext(project, fromUserName, toUserName);
			var body = RenderPartialViewToString("~/Views/Email/SendEmailToGroup.cshtml", context);
			SendEmail(subject, body, toEmailAddress);
		}

		public void SendEmailToProposer(string subject, Project project, string fromUserName, string toUserName, string toEmailAddress)
		{
			var context = getEmailCallbackContext(project, fromUserName, toUserName);
			var body = RenderPartialViewToString("~/Views/Email/SendEmailToProposer.cshtml", context);
			SendEmail(subject, body, toEmailAddress);
		}

		public void ProjectHasPickedUpEmail(string subject, Project project, string fromUserName, string toUserName, string toEmailAddress)
		{
			var context = getEmailCallbackContext(project, fromUserName, toUserName);
			var body = RenderPartialViewToString("~/Views/Email/ProjectHasPickedUpEmail.cshtml", context);
			SendEmail(subject, body, toEmailAddress);
		}

		public void ProjectInvitationEmailToStudent(string subject, Project project, string fromUserName, string toUserName, string toEmailAddress)
		{
			var context = getEmailCallbackContext(project, fromUserName, toUserName);

			var body = RenderPartialViewToString("~/Views/Email/ProjectInvitationEmailToStudent.cshtml", context);
			SendEmail(subject, body, toEmailAddress);
		}

		public void ProjectInvitationEmailToSupervisor(string subject, Project project, string fromUserName, string toUserName, string toEmailAddress)
		{
			var context = getEmailCallbackContext(project, fromUserName, toUserName);

			var body = RenderPartialViewToString("~/Views/Email/ProjectInvitationEmailToSupervisor.cshtml", context);
			SendEmail(subject, body, toEmailAddress);
		}

        #endregion

        #region Helper
 
        private void SendEmail(string subject, string body, string emailAddress)
		{
			var credentialUserName = ConfigurationManager.AppSettings["emailFrom"];
			var sentFrom = ConfigurationManager.AppSettings["emailFrom"];
			var pwd = ConfigurationManager.AppSettings["emailPassword"];
			var SMTP = ConfigurationManager.AppSettings["servidorSMTP"];
			var port = ConfigurationManager.AppSettings["SMTPPort"];

			// Configure the client:
			SmtpClient client = new SmtpClient(SMTP);
			client.EnableSsl = true;    //need change for smtp
			client.Port = Convert.ToInt32(port);           //need change for smtp
			client.DeliveryMethod = SmtpDeliveryMethod.Network;
			client.UseDefaultCredentials = false;
			client.Credentials = new NetworkCredential(credentialUserName, pwd);

			// Create the message:
			var mail = new MailMessage(sentFrom, emailAddress);
			mail.Subject = subject;
			mail.Body = body;

			#region formatter
			string text = string.Format("{1} \n\n  === Do not reply this Email ===  ", subject, body);
			string html = string.Format("{1} \n\n  === Do not reply this Email ===  ", subject, body);
			//		string html = ""; // "Please confirm your account by clicking this link: <a href=\"" + message.Body + "\">Click me</a><br/>";
			//		html += HttpUtility.HtmlEncode(@"Welcome! Please click on the or copy the following link on the browser: " +body);
			#endregion

			mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
			mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));
			client.Send(mail);
            //client.SendMailAsync(mail);  //error
        }

		private string RenderPartialViewToString(string templatePath, DynamicViewBag context)
		{
			string template = File.ReadAllText(HostingEnvironment.MapPath(templatePath));
			string renderedText = Engine.Razor.RunCompile(template, templatePath, null, context);
			return renderedText;
		}

		private DynamicViewBag getEmailCallbackContext(Project project, string fromUserName, string toUserName)
		{
			String strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
			String strUrl = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
			string publisherType = project.PublisherType == "ExtSupervisor" ? "ExternalSupervisor" : project.PublisherType;

			var context = new DynamicViewBag();
			context.AddValue("PublisherType", publisherType);
			context.AddValue("ToUserName", toUserName);
			context.AddValue("FromUserName", fromUserName);
			context.AddValue("ProjectID", project.ID);
			context.AddValue("ProjectTitle", project.Title);
			context.AddValue("CallbackURL", strUrl);

			return context;
		}

		#endregion
	}
}
