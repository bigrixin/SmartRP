using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace SmartRP.Domain.Service
{
	public interface IUserService
	{
		void CreateStudent(IdentityUser aspNetUser);
		void CreateSupervisor(IdentityUser aspNetUser);
		void CreateCoSupervisor(IdentityUser aspNetUser);
		void CreateExternalSupervisor(IdentityUser aspNetUser);
		void CreateCoordinator(IdentityUser aspNetUser);
		User FindUser(int? id);
		List<User> GetAllUser();
		string GetUserRoleByID(int userID);
		string GetCurrentRole(string loginIdentityID);
		bool IsNewUser(string loginIdentityName);
		User GetUserIDByEmail(string loginIdentityName);
		int GetUserIDByIdentityID(string loginIdentityID);

		Student GetCurrentStudent(int userID);
		Supervisor GetCurrentSupervisor(int userID);
		CoSupervisor GetCurrentCoSupervisor(int userID);
		ExternalSupervisor GetCurrentExternalSupervisor(int userID);
		Coordinator GetCurrentCoordinator(int userID);

	}
}