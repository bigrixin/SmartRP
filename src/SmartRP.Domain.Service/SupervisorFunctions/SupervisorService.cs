using SmartRP.Domain;
using SmartRP.Infrastructure;
using SmartRP.Infrastructure.Auth;
using System;
using System.Linq;

namespace SmartRP.Domain.Service
{
	public class SupervisorService : ISupervisorService
	{

		#region Fields

		private readonly IWriteEntities _entities;
		private readonly ILoginService _loginServices;

		#endregion

		#region Ctor

		public SupervisorService(IWriteEntities entities, ILoginService loginServices)
		{
			this._entities = entities;
			this._loginServices = loginServices;
		}

		#endregion

		#region ISupervisorService

		public Supervisor GetLoggedInSupervisor()
		{
			var identityId = this._loginServices.GetCurrentLoginIdentityID();
			return this._entities.Get<Supervisor>(c => c.LoginIdentityID == identityId).First();
		}

		public Supervisor GetCurrentSupervisor(int userId)
		{
			return this._entities.Get<Supervisor>(c => c.ID == userId).First();
		}

		public void UpdateProfile(Supervisor supervisor)
		{
			if (supervisor == null)
				throw new ArgumentNullException("supervisor");

			var now = DateTime.Now;
			supervisor.UpdatedAt = now;
			this._entities.Update(supervisor);
			this._entities.Save();
		}

	 

		#endregion

	}
}