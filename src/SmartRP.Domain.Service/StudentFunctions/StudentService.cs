using SmartRP.Domain;
using SmartRP.Infrastructure;
using SmartRP.Infrastructure.Auth;
using System.Linq;

namespace SmartRP.Domain.Service
{
	public class StudentService : IStudentService
	{

		#region Fields

		private readonly IWriteEntities _entities;
		private readonly ILoginService _loginServices;

		#endregion

		#region Ctor

		public StudentService(IWriteEntities entities, ILoginService loginServices)
		{
			this._entities = entities;
			this._loginServices = loginServices;
		}

		#endregion

		#region IStudentService

		public Student GetLoggedInStudent()
		{
			var identityId = this._loginServices.GetCurrentLoginIdentityID();
			return this._entities.Get<Student>(c => c.LoginIdentityID == identityId).FirstOrDefault();
		}

		public Student GetCurrentStudent(int userId)
		{
			return this._entities.Get<Student>(c => c.ID == userId).SingleOrDefault();
		}
 
		#endregion

	}
}