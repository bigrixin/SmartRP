using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SmartRP.Domain;
using SmartRP.Infrastructure;
using System;
using PagedList;
using SmartRP.Infrastructure.Data;
using SmartRP.Domain.Service;
using AutoMapper;
using System.Net;

namespace SmartRP.Controllers
{

	[Authorize]
	public class KeywordsController : Controller
	{

		#region Fields

		private readonly IWriteEntities _entities;
		private readonly IStudentService _studentServices;
		private readonly ISupervisorService _supervisorServices;
		private readonly ICoordinatorService _coordinatorServices;
		private readonly IUserService _userServices;
		private readonly ICommonService _commonServices;
		private readonly DbData _dbData;
		private readonly IProjectService _projectServices;
		private const int PageSize = 10;
		private readonly IMapper _mapper;

		#endregion

		#region Ctor

		public KeywordsController(IWriteEntities entities, IStudentService studentServices, ISupervisorService supervisorServices, ICoordinatorService coordinatorServices, IUserService userServices, ICommonService commonServices, DbData dbData, IProjectService projectServices, IMapper mapper)
		{
			_entities = entities;
			_studentServices = studentServices;
			_supervisorServices = supervisorServices;
			_coordinatorServices = coordinatorServices;
			_userServices = userServices;
			_commonServices = commonServices;
			_dbData = dbData;
			_projectServices = projectServices;
			_mapper = mapper;
		}

		#endregion

		[HttpGet]
		[Authorize(Roles = "Coordinator")]
		public ActionResult MyList(string currentFilter, string searchString, int? page)
		{
			if (searchString != null)
				page = 1;
			else
				searchString = currentFilter;
			ViewBag.CurrentFilter = searchString;
			int pageIndex = 1;
			pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

			var keywordsVM = new List<KeywordViewModel>();
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			var keywords = currentCoordinator.GetKeywords();
			keywords.ToList().ForEach(j =>
			{
				var vm = _mapper.Map<Keyword, KeywordViewModel>(j);
				keywordsVM.Add(vm);
			});

			ViewBag.UserId = _userServices.GetUserIDByEmail(User.Identity.Name).ID;
			ViewBag.TypeOfUser = _userServices.GetCurrentRole(User.Identity.Name);
			int pageNumber = (page ?? 1);
			return View(keywordsVM.ToPagedList(pageNumber, PageSize));
		}

		// GET: Keyword
		[HttpGet]
		[Authorize(Roles = "Coordinator")]
		public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
		{
			ViewBag.CurrentSort = sortOrder;
			ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
			ViewBag.DescriptionSortParm = sortOrder ==
												 "description" ? "descriptionDesc" : "description";
			if (searchString != null)
				page = 1;
			else
				searchString = currentFilter;

			ViewBag.CurrentFilter = searchString;
			int pageIndex = 1;
			pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

			IEnumerable<Keyword> keywords = _commonServices.GetKeywordList();

			if (!String.IsNullOrEmpty(searchString))
			{
				keywords = keywords.Where(s => s.Description != null);
				keywords = keywords.Where(s => s.Title.ToLower().Contains(searchString.ToLower())
																|| s.Description.ToLower().Contains(searchString.ToLower()));
			}

			switch (sortOrder)
			{
				case "name_desc":
					keywords = keywords.OrderByDescending(s => s.Title);
					break;
				case "description":
					keywords = keywords.OrderBy(s => s.Description).ToPagedList(pageIndex, PageSize);
					break;
				case "descriptionDesc":
					keywords = keywords.OrderByDescending(s => s.Description).ToPagedList(pageIndex, PageSize);
					break;
				default:  // Name ascending 
					keywords = keywords.OrderBy(s => s.Title);
					break;
			}

			var interestVM = new List<KeywordViewModel>();
			keywords.ToList().ForEach(j =>
			{
				var vm = _mapper.Map<Keyword, KeywordViewModel>(j);
				interestVM.Add(vm);
			});

			ViewBag.TypeOfUser = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.UserId = _userServices.GetUserIDByEmail(User.Identity.Name).ID;

			int pageNumber = (page ?? 1);
			return View(interestVM.ToPagedList(pageNumber, PageSize));
		}

		// GET: Keywords/Create
		public ActionResult Create()
		{
			KeywordViewModel model = new KeywordViewModel();
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			if (userType != "Coordinator")
				return RedirectToAction("MyAccount/", userType);
			return View(model);
		}

		// POST: Keywords/Create
		[HttpPost]   //, Route("keywords/create")
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Coordinator")]
		public ActionResult Create(KeywordViewModel model)
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			if (currentCoordinator == null || userType != "Coordinator")
				return RedirectToAction("MyAccount/", userType);

			if (ModelState.IsValid)
			{
				var keyword = _coordinatorServices.AddKeyword(currentCoordinator, model);

				if (keyword == null)
				{
					ViewBag.Error = "Error: Added Keyword has exist !";
					return View(model);
				}
			}
			return RedirectToAction("MyList");
		}

		// GET: Keywords/Edit/5
		[Authorize(Roles = "Coordinator")]
		public ActionResult Edit(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			if (currentCoordinator == null || userType != "Coordinator")
				return RedirectToAction("MyAccount/", userType);
			var keyword = currentCoordinator.GetKeywordByID((int)id);
			var vm = _mapper.Map<Keyword, KeywordViewModel>(keyword);
			return View(vm);
		}

		// POST: Keyword/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(KeywordViewModel model)
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			if (currentCoordinator == null || userType != "Coordinator")
				return RedirectToAction("MyAccount/", userType);

			if (ModelState.IsValid && currentCoordinator.ID == model.CoordinatorID)
			{
				var keyword = _coordinatorServices.UpdateKeyword(currentCoordinator, model);
				if (keyword == null)
				{
					ViewBag.Error = "This Keyword does not existed!";
					return View(model);
				}
				return RedirectToAction("mylist");
			}
			return View(model);
		}

		// GET: Keywords/Delete/5
		[Authorize(Roles = "Coordinator")]
		public ActionResult Delete(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			if (currentCoordinator == null || userType != "Coordinator")
				return RedirectToAction("MyAccount/", userType);
			var keyword = currentCoordinator.GetKeywordByID((int)id);

			int userUsedKeyword = _coordinatorServices.GetUserSelectedKeywordsCount(keyword.ID);
			int projectUsedKeyword = _coordinatorServices.GetProjectSelectedKeywordsCount(keyword.ID);

			if (userUsedKeyword != 0 || projectUsedKeyword != 0)
				return RedirectToAction("ErrorAlert", "Error", new
				{
					errorTitle = "Delete keyword error !",
					errorContent = "There are " + userUsedKeyword + " users has selected the keyword in the profile. \n\n" +
													"There are " + userUsedKeyword + " projects has selected the keyword"
				});


			var vm = _mapper.Map<Keyword, KeywordViewModel>(keyword);
			return View(vm);
		}

		// POST: Keyword/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			if (currentCoordinator == null || userType != "Coordinator")
				return RedirectToAction("MyAccount/", userType);

			ViewBag.TypeOfUser = userType;

			var keyword = _coordinatorServices.DeleteKeyword(currentCoordinator.ID, id);
			if (keyword == null)
			{
				ViewBag.Error = "This Keyword does not existed!";
				return RedirectToAction("delete", id);
			}
			return RedirectToAction("mylist");
		}

		//[HttpGet]
		//public ActionResult MatchInterest(string currentFilter, string searchString, int? page, string index)
		//{
		//	int PageSize = 20;
		//	if (searchString != null)
		//		page = 1;
		//	else
		//		searchString = currentFilter;

		//	ViewBag.CurrentFilter = searchString;
		//	int pageIndex = 1;
		//	pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

		//	var userType = this._userServices.GetCurrentRole(User.Identity.Name);
		//	ViewBag.TypeOfUser = userType;
		//	ViewBag.EnrollSemester = this._commonServices.IsEnrolledUser(User.Identity.Name);
		//	int userId = 0;
		//	switch (userType)
		//	{
		//		case "Student":
		//			var student = this._studentServices.GetLoggedInStudent();
		//			if (student == null)
		//				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "User error", errorContent = "Can not found student !" });
		//			userId = student.ID;
		//			break;
		//		case "Supervisor":
		//			var supervisor = this._supervisorServices.GetLoggedInSupervisor();
		//			if (supervisor == null)
		//				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "User error", errorContent = "Can not found supervisor !" });
		//			userId = supervisor.ID;
		//			break;
		//		case "Coordinator":
		//			var coordinator = this._coordinatorServices.GetLoggedInCoordinator();
		//			if (coordinator == null)
		//				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "User error", errorContent = "Can not found coordinator !" });
		//			userId = coordinator.ID;
		//			break;
		//	}
		//	List<MatchKeywordViewModel> userSelectedInterestList;
		//	if (index == "byuser")
		//	{
		//		var currentTerm = this._commonServices.GetCurrentTerm();
		//		userSelectedInterestList = this._dbData.GetMatchingKeywordsListByUser(currentTerm.ID, userId);
		//	}
		//	else
		//		userSelectedInterestList = this._commonServices.GetMatchingKeywordsListByKeywords(userId);

		//	int pageNumber = (page ?? 1);
		//	return View(userSelectedInterestList.ToPagedList(pageNumber, PageSize));
		//}

		//[HttpGet]
		//public ActionResult MatchInterestByUser(string currentFilter, string searchString, int? page, string index)
		//{
		//	int PageSize = 20;
		//	if (searchString != null)
		//		page = 1;
		//	else
		//		searchString = currentFilter;

		//	ViewBag.CurrentFilter = searchString;
		//	int pageIndex = 1;
		//	pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

		//	var userType = this._userServices.GetCurrentRole(User.Identity.Name);
		//	ViewBag.TypeOfUser = userType;
		//	ViewBag.EnrollSemester = this._commonServices.IsEnrolledUser(User.Identity.Name);
		//	int userId = 0;
		//	switch (userType)
		//	{
		//		case "Student":
		//			var student = this._studentServices.GetLoggedInStudent();
		//			if (student == null)
		//				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "User error", errorContent = "Can not found student !" });
		//			userId = student.ID;
		//			break;
		//		case "Supervisor":
		//			var supervisor = this._supervisorServices.GetLoggedInSupervisor();
		//			if (supervisor == null)
		//				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "User error", errorContent = "Can not found supervisor !" });
		//			userId = supervisor.ID;
		//			break;
		//		case "Coordinator":
		//			var coordinator = this._coordinatorServices.GetLoggedInCoordinator();
		//			if (coordinator == null)
		//				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "User error", errorContent = "Can not found coordinator !" });
		//			userId = coordinator.ID;
		//			break;
		//	}
		//	List<MatchKeywordViewModel> userSelectedInterestList;
		//	if (index == "byuser")
		//	{
		//		var currentTerm = this._commonServices.GetCurrentTerm();
		//		userSelectedInterestList = this._dbData.GetMatchingKeywordsListByUser(currentTerm.ID, userId);
		//	}
		//	else
		//		userSelectedInterestList = this._commonServices.GetMatchingKeywordsListByKeywords(userId);

		//	int pageNumber = (page ?? 1);
		//	return View(userSelectedInterestList.ToPagedList(pageNumber, PageSize));
		//}

		//public ActionResult FindProjectMates(int id, string currentFilter, string searchString, int? page)
		//{

		//	int PageSize = 20;
		//	if (searchString != null)
		//		page = 1;
		//	else
		//		searchString = currentFilter;

		//	ViewBag.CurrentFilter = searchString;
		//	int pageIndex = 1;
		//	pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

		//	var userType = this._userServices.GetCurrentRole(User.Identity.Name);
		//	ViewBag.TypeOfUser = userType;
		//	ViewBag.EnrollSemester = this._commonServices.IsEnrolledUser(User.Identity.Name);
		//	int userId = 0;
		//	switch (userType)
		//	{
		//		case "Student":
		//			var student = this._studentServices.GetLoggedInStudent();
		//			if (student == null)
		//				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "User error", errorContent = "Can not found student !" });
		//			userId = student.ID;
		//			break;
		//		case "Supervisor":
		//			var supervisor = this._supervisorServices.GetLoggedInSupervisor();
		//			if (supervisor == null)
		//				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "User error", errorContent = "Can not found supervisor !" });
		//			userId = supervisor.ID;
		//			break;
		//		case "Coordinator":
		//			var coordinator = this._coordinatorServices.GetLoggedInCoordinator();
		//			if (coordinator == null)
		//				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "User error", errorContent = "Can not found coordinator !" });
		//			userId = coordinator.ID;
		//			break;
		//	}
		//	List<MatchKeywordViewModel> userSelectedInterestList;

		//	var currentTerm = this._commonServices.GetCurrentTerm();
		//	userSelectedInterestList = this._dbData.FindProjectMatchingKeywordsStudents(currentTerm.ID, id);
		//	ViewBag.ProjectID = id;
		//	ViewBag.ProjectTitle = this._projectServices.GetCurrentProject(id).Title;

		//	int pageNumber = (page ?? 1);
		//	return View(userSelectedInterestList.ToPagedList(pageNumber, PageSize));
		//}

		//public ActionResult FindProjectSupervisor(int id, string currentFilter, string searchString, int? page)
		//{
		//	int PageSize = 20;
		//	if (searchString != null)
		//		page = 1;
		//	else
		//		searchString = currentFilter;

		//	ViewBag.CurrentFilter = searchString;
		//	int pageIndex = 1;
		//	pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

		//	var userType = this._userServices.GetCurrentRole(User.Identity.Name);
		//	ViewBag.TypeOfUser = userType;
		//	ViewBag.EnrollSemester = this._commonServices.IsEnrolledUser(User.Identity.Name);
		//	int userId = 0;
		//	switch (userType)
		//	{
		//		case "Student":
		//			var student = this._studentServices.GetLoggedInStudent();
		//			if (student == null)
		//				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "User error", errorContent = "Can not found student !" });
		//			userId = student.ID;
		//			break;
		//		case "Supervisor":
		//			var supervisor = this._supervisorServices.GetLoggedInSupervisor();
		//			if (supervisor == null)
		//				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "User error", errorContent = "Can not found supervisor !" });
		//			userId = supervisor.ID;
		//			break;
		//		case "Coordinator":
		//			var coordinator = this._coordinatorServices.GetLoggedInCoordinator();
		//			if (coordinator == null)
		//				return RedirectToAction("ErrorAlert", "Error", new { errorTitle = "User error", errorContent = "Can not found coordinator !" });
		//			userId = coordinator.ID;
		//			break;
		//	}
		//	List<MatchKeywordViewModel> userSelectedInterestList;

		//	var currentTerm = this._commonServices.GetCurrentTerm();
		//	userSelectedInterestList = this._dbData.FindProjectMatchingKeywordsSupervisors(currentTerm.ID, id);
		//	ViewBag.ProjectID = id;
		//	ViewBag.ProjectTitle = this._projectServices.GetCurrentProject(id).Title;

		//	int pageNumber = (page ?? 1);
		//	return View(userSelectedInterestList.ToPagedList(pageNumber, PageSize));
		//}

		//#region Helper

		//private List<KeywordViewModel> GetStudentInterestVM(List<KeywordViewModel> interestVM)
		//{
		//	var student = this._studentServices.GetLoggedInStudent();

		//	if (student != null)
		//	{
		//		var interests = student.AddedInterests;

		//		interests.ToList().ForEach(j =>
		//		{
		//			var vm = MapKeywordViewModel(j);
		//			interestVM.Add(vm);
		//		});
		//		return interestVM;
		//	}
		//	return null;
		//}

		//private List<KeywordViewModel> GetSupervisorInterestVM(List<KeywordViewModel> interestVM)
		//{
		//	var supervisor = this._supervisorServices.GetLoggedInSupervisor();

		//	if (supervisor != null)
		//	{
		//		var interests = supervisor.AddedInterests;

		//		interests.ToList().ForEach(j =>
		//		{
		//			var vm = MapKeywordViewModel(j);
		//			interestVM.Add(vm);
		//		});
		//		return interestVM;
		//	}
		//	return null;
		//}

		//private List<KeywordViewModel> GetCoordinatorInterestVM(List<KeywordViewModel> interestVM)
		//{
		//	var coordinator = this._coordinatorServices.GetLoggedInCoordinator();

		//	if (coordinator != null)
		//	{
		//		var interests = coordinator.AddedInterests;

		//		interests.ToList().ForEach(j =>
		//		{
		//			var vm = MapKeywordViewModel(j);
		//			interestVM.Add(vm);
		//		});
		//		return interestVM;
		//	}
		//	return null;
		//}

		//private KeywordViewModel MapKeywordViewModel(Keyword interest)
		//{
		//	KeywordViewModel model = new KeywordViewModel()
		//	{
		//		ID = interest.ID,
		//		Title = interest.Title,
		//		Description = interest.Description,
		//		//Status = interest.Status,
		//		//StudentId = interest.StudentId.GetValueOrDefault(),
		//		//SupervisorId = interest.SupervisorId.GetValueOrDefault(),
		//		//CoordinatorId = interest.CoordinatorId.GetValueOrDefault()
		//	};
		//	return model;
		//}

		//#endregion


	}
}
