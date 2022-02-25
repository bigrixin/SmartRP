using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SmartRP.Domain;
using SmartRP.Domain.Service;
using AutoMapper;
using System;
using PagedList;

namespace SmartRP.Controllers
{

	[Authorize(Roles = "Coordinator")]
	public class TermsController : Controller
	{

		#region Fields

		private readonly ICoordinatorService _coordinatorServices;
		private readonly IUserService _userServices;
		private readonly ICommonService _commonServices;
		private readonly IMapper _mapper;
		private const int PageSize = 10;

		#endregion

		#region Ctor

		public TermsController(ICoordinatorService coordinatorServices, IUserService userServices, ICommonService commonServices, IMapper mapper)
		{
			_coordinatorServices = coordinatorServices;
			_userServices = userServices;
			_commonServices = commonServices;
			_mapper = mapper;
		}

		#endregion

		#region Action

		public ActionResult Index()
		{
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			TermViewModel model = new TermViewModel();
			List<TermViewModel> termVM = new List<TermViewModel>();
			if (currentCoordinator != null)
			{
				ViewBag.CoordinatorID = currentCoordinator.ID;
				var terms = _commonServices.GetTermList().OrderByDescending(a => a.StartAt).ToList();
				terms.ForEach(j =>
				{
					var vm = _mapper.Map<Term, TermViewModel>(j);
					termVM.Add(vm);
				});
			}
			ViewBag.CoordinatorID = currentCoordinator.ID;
			ViewBag.TypeOfUser = _userServices.GetCurrentRole(User.Identity.Name);
			return View(termVM);
		}

		// GET: Terms/Create
		public ActionResult Create()
		{
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			TermEditModel model = _mapper.Map<Term, TermEditModel>(new Term(currentCoordinator.ID));
			ViewBag.TypeOfUser = _userServices.GetCurrentRole(User.Identity.Name);
			return View(model);
		}

		// POST: Terms/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(TermEditModel model)
		{
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			if (currentCoordinator != null && ModelState.IsValid)
				_coordinatorServices.AddTerm(currentCoordinator, model);

			return RedirectToAction("Index");
		}

		// GET: Terms/Edit/5
		public ActionResult Edit(int? id)
		{
			ViewBag.TypeOfUser = _userServices.GetCurrentRole(User.Identity.Name);
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			if (currentCoordinator == null || id == null)
				return RedirectToAction("Index");
			Term term = currentCoordinator.GetTerm((int)id);
			if (term == null)
				return RedirectToAction("Index");
			TermEditModel model = _mapper.Map<Term, TermEditModel>(term);
			return View(model);
		}

		// POST: Terms/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(TermEditModel model)
		{
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			if (currentCoordinator != null && ModelState.IsValid && model.CoordinatorID == currentCoordinator.ID)
			{
				_coordinatorServices.UpdateTerm(currentCoordinator, model);
				return RedirectToAction("Index");
			}
			return View(model);
		}

		// GET: Terms/Delete/5
		public ActionResult Delete(int? id)
		{
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			if (id == null || currentCoordinator == null)
				return RedirectToAction("Index");
			Term term = currentCoordinator.GetTerm((int)id);
			if (term == null)
				return RedirectToAction("Index");
			TermViewModel model = _mapper.Map<Term, TermViewModel>(term);
			var enrolledStudents = _commonServices.GetEnrolledStudentsBySemester(term);

      //need to count co-supervisor and ext-supervisor 
			if (enrolledStudents.Count() != 0)
				return RedirectToAction("ErrorAlert", "Error", new
				{
					errorTitle = "Delete semester error !",
					errorContent = "There are " + enrolledStudents.Count() + " students has enrolled the semester."
				});
			ViewBag.TypeOfUser = _userServices.GetCurrentRole(User.Identity.Name);
			return View(model);
		}

		// POST: Terms/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Coordinator currentCoordinator = this.GetLoggedInUser() as Coordinator;
			if (currentCoordinator != null)
				_coordinatorServices.DeleteTerm(currentCoordinator, id);

			return RedirectToAction("Index");
		}

		#endregion

		#region Enrol

		[HttpGet, Route("terms/enrolled-students")]
		public ActionResult EnrolledStudents(int? id, string currentFilter, string searchString, int? page)
		{
			if (searchString != null)
				page = 1;
			else
				searchString = currentFilter;

			ViewBag.CurrentFilter = searchString;
			int pageIndex = 1;
			pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

			ViewBag.SubjectID = id;
			ViewBag.SubjectName = _commonServices.GetSemesterBySubjectID((int)id).TermName +" "+ _commonServices.GetSubjectNameByID((int)id);
			var students =_commonServices.GetCurrentSubjectStudents((int)id).ToList();

			if (!String.IsNullOrEmpty(searchString))
			{
				students = students.Where(s => s.Firstname.ToLower().Contains(searchString.ToLower()) ||
								s.Lastname.ToLower().Contains(searchString.ToLower()) ||
								s.StudentID.ToLower().Contains(searchString.ToLower()) ||
								s.Email.ToLower().Contains(searchString.ToLower())).ToList();
			}

			var studentVMList = new List<StudentViewModel>();
			if (students != null)
			{
				students.ForEach(j =>
				{
					var vm = _mapper.Map<Student, StudentViewModel>(j); 
					studentVMList.Add(vm);
				});
			}

			string userType = _userServices.GetCurrentRole(User.Identity.Name);
			ViewBag.TypeOfUser = userType;
			int pageNumber = (page ?? 1);
			return View(studentVMList.ToPagedList(pageNumber, PageSize));
		}


		#endregion
	}
}
