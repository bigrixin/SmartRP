using AutoMapper;
using SmartRP.Infrastructure;
using SmartRP.Infrastructure.Auth;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRP.Domain.Service
{
	public class CoordinatorService : ICoordinatorService
	{

		#region Fields

		private readonly IWriteEntities _writeEntities;
		private readonly ILoginService _loginServices;
		private readonly IReadEntities _readEntities;
		private readonly ICommonService _commonServices;
		private readonly IMapper _mapper;

		#endregion

		#region Ctor

		public CoordinatorService(IWriteEntities writeEntities, ILoginService loginServices, IReadEntities readEntities, ICommonService commonServices, IMapper mapper)
		{
			this._writeEntities = writeEntities;
			this._readEntities = readEntities;
			this._loginServices = loginServices;
			this._commonServices = commonServices;
			this._mapper = mapper;
		}

		#endregion

		#region Profile

		public Coordinator GetLoggedInCoordinator()   ///may not use
		{
			var identityId = this._loginServices.GetCurrentLoginIdentityID();
			return this._readEntities.Get<Coordinator>(c => c.LoginIdentityID == identityId).FirstOrDefault();
		}

		public Coordinator GetCurrentCoordinator(int userId)
		{
			return this._readEntities.Get<Coordinator>(c => c.ID == userId).First();
		}

		#endregion

		#region Keyword

		public int GetUserSelectedKeywordsCount(int keywordID)
		{
			int userUsedKeyword = 0;
			var usedKeyword = _readEntities.Get<UserKeyword>(a => a.KeywordID == keywordID);
			if (usedKeyword != null)
				userUsedKeyword = usedKeyword.Count();
			return userUsedKeyword;
		}

		public int GetProjectSelectedKeywordsCount(int keywordID)
		{
			int projectUsedKeyword = 0;
			var usedKeyword = _readEntities.Get<ProjectKeyword>(a => a.KeywordID == keywordID);
			if (usedKeyword != null)
				projectUsedKeyword = usedKeyword.Count();
			return projectUsedKeyword;
		}


		public List<Keyword> GetSelectedKeywords(int userID)
		{
			var currentCoodinator = GetCurrentCoordinator(userID);
			return currentCoodinator.GetKeywords();
		}

		public Keyword AddKeyword(Coordinator currentCoordinator, KeywordViewModel model)
		{
			var hasExist = this._readEntities.Get<Keyword>().Where(a => a.Title == model.Title).SingleOrDefault();
			if (hasExist != null)
				return null;
 
			Keyword newKeyword = new Keyword(model.Title, model.Description);
			currentCoordinator.AddKeyword(newKeyword);
			this._writeEntities.Update(currentCoordinator);
			this._writeEntities.Save();

			return newKeyword;
		}

		public Keyword UpdateKeyword(Coordinator currentCoordinator, KeywordViewModel model)
		{
			var keyword = currentCoordinator.GetKeywordByID(model.ID);
			if (keyword == null)
				return null;

			keyword = _mapper.Map(model, keyword);
			keyword.UpdatedAt = DateTime.Now;
			this._writeEntities.Update(currentCoordinator);
			this._writeEntities.Save();

			return keyword;
		}

		public Keyword DeleteKeyword(int coordinatorID, int keywordID)
		{
			var coordinator = GetCurrentCoordinator(coordinatorID);
			if (coordinator == null)
				throw new ArgumentNullException("Coordinator ID does not exist !");

			Keyword keyword = coordinator.GetKeywordByID(keywordID);
			if (keyword == null)
				return null;
			coordinator.DeleteKeyword(keyword);
			this._writeEntities.Delete(keyword);
			this._writeEntities.Update(coordinator);
			this._writeEntities.Save();
			return keyword;
		}

		#endregion

		#region Term

		public void AddTerm(Coordinator currentCoordinator, TermEditModel model)
		{
			Term term = new Term(currentCoordinator.ID);
			term = this._mapper.Map<TermEditModel, Term>(model);
			term.Subjects= this._commonServices.GetSubjectsFromModel(model.CheckBoxSubjectNames, model.ID);

			term = currentCoordinator.AddTerm(term);
			if (term != null)
			{
				this._writeEntities.Update(currentCoordinator);
				this._writeEntities.Save();
			}
		}

		public void UpdateTerm(Coordinator currentCoordinator, TermEditModel model)
		{
			var term = currentCoordinator.GetTerm(model.ID);
			term = this._mapper.Map(model, term);

			var oldSubjects = term.GetSubjects();

			var newSubjects = this._commonServices.GetSubjectsFromModel(model.CheckBoxSubjectNames, term.ID);
			foreach (var subject in newSubjects)
			{
				term.AddSubject(subject);
			}

			currentCoordinator.UpdateTerm(term);
			this._writeEntities.Update(currentCoordinator);
			this._writeEntities.Save();

			var subjects = this._readEntities.Get<Subject>(a => a.TermID == model.ID);

			foreach (var subject in subjects)
			{
				bool find = false;
				foreach (var item in newSubjects)
				{
					if (item.SubjectName == subject.SubjectName)
					{
						find = true;
						break;
					}
				}
				if (!find)
				{
					this._writeEntities.Delete(subject);
					this._writeEntities.Update(currentCoordinator);
					this._writeEntities.Save();
				}
			}
		}

		public void DeleteTerm(Coordinator currentCoordinator, int termID)
		{
			Term term = currentCoordinator.Terms.SingleOrDefault(i => i.ID == termID);
			if (term != null)
			{
				currentCoordinator.Terms.Remove(term);
				this._writeEntities.Delete(term);
				this._writeEntities.Update(currentCoordinator);
				this._writeEntities.Save();
			}
		}

		#endregion

	}
}