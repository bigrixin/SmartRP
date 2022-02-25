using AutoMapper;
using SmartRP.Domain;
using SmartRP.Domain.Service;
using SmartRP.Infrastructure.Data;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SmartRP.Controllers
{
	[Authorize]
	public class CoordinatorController : Controller
	{
		// GET: Coordinator

		#region Fields

		private readonly ICommonService _commonServices;
		private readonly IProjectService _projectServices;
		private readonly DbData _dbData;
		private readonly IMapper _mapper;

		#endregion

		#region Ctor

		public CoordinatorController(ICommonService commonServices, IProjectService projectServices, DbData dbData, IMapper mapper)
		{
			_commonServices = commonServices;
			_projectServices = projectServices;
			_dbData = dbData;
			_mapper = mapper;
		}

		#endregion

		#region Menu

		[Authorize(Roles = "Coordinator")]
		public ActionResult Menu()
		{
			return PartialView("CoordinatorMenu");
		}

		#endregion

		#region Profile

		[Authorize(Roles = "Coordinator")]
		public ActionResult MyAccount()
		{
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			CoordinatorViewModel model = new CoordinatorViewModel();
			if (currentCoordinator != null)
			{
				model = _mapper.Map<Coordinator, CoordinatorViewModel>(currentCoordinator);
				model.UserSelectedKeywords = _commonServices.GetUserSelectedKeywordsByUserID(currentCoordinator.ID);
			}
			return View(model);
		}

		[Authorize(Roles = "Coordinator")]
		[HttpGet, Route("coordinator/update-profile")]
		public ActionResult UpdateProfile(string userid)
		{
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			CoordinatorProfileEditModel model = new CoordinatorProfileEditModel();
			if (currentCoordinator != null && currentCoordinator.LoginIdentityID == userid)
			{
				model = _mapper.Map<Coordinator, CoordinatorProfileEditModel>(currentCoordinator);
				model.KeywordList = _commonServices.GetKeywordList();
				model.SelectedKeywords = _commonServices.GetUserSelectedKeywordsByUserID(currentCoordinator.ID);
			}
			return View(model);
		}

		[Authorize(Roles = "Coordinator")]
		[HttpPost, Route("coordinator/update-profile")]
		public ActionResult UpdateProfile(CoordinatorProfileEditModel model)
		{
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			if (currentCoordinator != null && ModelState.IsValid)
			{
				currentCoordinator = _mapper.Map(model, currentCoordinator);
				_commonServices.UpdateProfile(currentCoordinator, model.SelectedKeywordIDs);
			}
			return RedirectToAction("myaccount");
		}

		#endregion

		#region Term

		[HttpGet, Route("coordinator/manage-semester")]
		public ActionResult ManageSemester()
		{
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			if (currentCoordinator != null)
				return RedirectToAction("Index", "Terms", new { userId = currentCoordinator.ID });
			return View();
		}

		#endregion

		#region Keywords

		[HttpGet, Route("coordinator/manage-keywords")]
		public ActionResult ManageKeywords()
		{
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			if (currentCoordinator != null)
				return RedirectToAction("Index", "Keywords", new { userId = currentCoordinator.ID });
			return View();
		}

		#endregion

		#region Statistics

		[Authorize(Roles = "Coordinator")]
		[HttpGet, Route("coordinator/students-requests")]
		public ActionResult StudentsRequests()
		{
			ViewBag.TypeOfUser = "Coordinator";
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			Term currentTerm = _commonServices.GetCurrentOpenTerm();
			if (currentTerm == null)
				return RedirectToAction("manage-semester");
			List<RequestsViewModel> vm = _dbData.GetStudentsRequestsList(currentTerm.ID);

			return View(vm);
		}

		#endregion

		#region Pick up

		[HttpGet, Route("coordinator/posted-by-students")]
		public ActionResult PostedByStudents()
		{
			return RedirectToAction("posted-by-students", "projects");
		}

		#endregion

		#region Remove supervisor

		[HttpGet, Route("coordinator/remove-supervisor")]
		public ActionResult RemoveSupervisor(int? projectID)
		{
			ViewBag.TypeOfUser = "Coordinator";
			if (projectID == null)
				return View();
			ViewBag.ProjectID = projectID;
			ProjectViewModel model = _projectServices.MapperProjectToViewModel((int)projectID);
			if (model == null)
				return View();
			return View(model);
		}

		[HttpGet, Route("coordinator/confirm-to-remove-supervisor")]
		public ActionResult ConfirmRemoveSupervisor(int supervisorID, int projectID)
		{
			ViewBag.TypeOfUser = "Coordinator";

			_projectServices.RemoveSupervisor(supervisorID, projectID);
	    ViewBag.Keyword = projectID;
			return RedirectToAction("remove-supervisor", "coordinator", new { projectID = projectID });
		}

		#endregion

		#region Grade group

		[HttpGet, Route("coordinator/grade-group")]
		public ActionResult GradeGroup(int? groupID)
		{
			ViewBag.TypeOfUser = "Coordinator";
			if (groupID == null)
				return View();
			ViewBag.GroupID = groupID;
			GradeGroupViewModel model = _projectServices.MapperGradeGroupToViewModel((int)groupID);
			if (model == null)
				return View();
			ViewBag.Comments = model.Comments;
			model.Comments = "";
			return View(model);
		}

		[HttpPost, Route("coordinator/confirm-to-grade-group")]
		public ActionResult ConfirmGradeGroup(GradeGroupViewModel model)
		{
			ViewBag.TypeOfUser = "Coordinator";
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			if (currentCoordinator != null && ModelState.IsValid)
				_projectServices.GradeGroupByCoordinator(currentCoordinator.ID, model);
			ViewBag.Keyword = model.ID;
			return RedirectToAction("grade-group", "coordinator", new { groupID = model.ID });
		}

		#endregion
	}
}