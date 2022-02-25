using System.Collections.Generic;

namespace SmartRP.Domain.Service
{
	public interface ICoordinatorService
	{
		Coordinator GetLoggedInCoordinator();
		Coordinator GetCurrentCoordinator(int id);
		int GetUserSelectedKeywordsCount(int keywordID);
		int GetProjectSelectedKeywordsCount(int keywordID);
		List<Keyword> GetSelectedKeywords(int userID);
		Keyword AddKeyword(Coordinator currentCoordinator, KeywordViewModel model);
		Keyword UpdateKeyword(Coordinator currentCoordinator, KeywordViewModel model);
		Keyword DeleteKeyword(int coordinatorID, int keywordID);

		void AddTerm(Coordinator currentCoordinator, TermEditModel model);
		void UpdateTerm(Coordinator currentCoordinator, TermEditModel model);
		void DeleteTerm(Coordinator currentCoordinator, int termID);
	}
}



