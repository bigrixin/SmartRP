using SmartRP.Domain;

namespace SmartRP.Domain.Service
{
	public interface ISupervisorService
	{
		Supervisor GetLoggedInSupervisor();
		void UpdateProfile(Supervisor supervisor);
		Supervisor GetCurrentSupervisor(int userId);
 
	}
}





















