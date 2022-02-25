using SmartRP.Domain;
using SmartRP.Infrastructure.Auth;

namespace SmartRP.Domain.Service
{
	public interface IStudentService
	{
		Student GetLoggedInStudent();
		Student GetCurrentStudent(int userId);
	}
}





















