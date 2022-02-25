using System.Web.Mvc;
using SmartRP.Domain;
using SmartRP.Domain.Service;
using System.Collections.Generic;

namespace SmartRP.Controllers
{
	[Authorize]
	public class GroupsController : Controller
	{
 
		#region Fields

		private readonly IProjectService _projectServices;
		private readonly IUserService _userServices;

		#endregion

		#region Ctor
		public GroupsController(IProjectService projectServices, IUserService userServices)
		{
			_projectServices = projectServices;
			_userServices = userServices;
		}

		#endregion

		#region Manage Group

		[HttpGet, Route("groups/manage-project-group")]
		public ActionResult ManageProjectGroup()
		{
			User user = _userServices.GetUserIDByEmail(User.Identity.Name);
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			List<GroupViewModel> groupsVM = _projectServices.GetCurrentSemesterManagedGroupsVMList(userType, user.ID);
			if (groupsVM == null)
				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Group error !", errorContent = "Can not find current managed group !" });

			ViewBag.UerID = user.ID;
			return View(groupsVM);
		}

		#endregion


		#region Comment Report

		[HttpGet, Route("groups/comment-group-report")]
		public ActionResult CommentGroupReport(int projectID, int groupID, int reportID)
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			User user = _userServices.GetUserIDByEmail(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
 
			ReportCommentViewModel model = _projectServices.GetReportCommentVM(userType, user.ID, projectID, groupID, reportID);

			return View(model);
		}

		[HttpPost, Route("groups/comment-group-report")]
		public ActionResult CommentGroupReport(ReportCommentViewModel model)
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			User user = _userServices.GetUserIDByEmail(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			if (user != null && ModelState.IsValid)
			{
				if (!_projectServices.AddGroupCommentBySupervisors(userType, user.ID, model))
					return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "Report comment error !", errorContent = "Can not find current managed group report!" });
			}
			return RedirectToAction("manage-project-group");
		}

		#endregion

		#region Grade

		[HttpGet, Route("groups/mark-project-group")]
		public ActionResult MarkProjectGroup()
		{
 
			return RedirectToAction("UpdateInformation", "error");
		}

		#endregion


		#region Helper


		#endregion
	}
}
