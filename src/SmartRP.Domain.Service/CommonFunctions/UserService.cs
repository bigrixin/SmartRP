using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SmartRP.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRP.Domain.Service
{
	public class UserService : IUserService
	{

		#region Fields

		private readonly IWriteEntities _entities;

		private readonly UserManager<IdentityUser> _userManager;

		#endregion

		#region Ctor

		public UserService(UserManager<IdentityUser> userManager, IWriteEntities entities)
		{
			_userManager = userManager;
			_entities = entities;
		}

		#endregion

		#region User Service

		public void CreateStudent(IdentityUser aspNetUser)
		{
			if (aspNetUser == null)
				throw new ArgumentNullException("aspNetUser");

			var now = DateTime.Now;
			var user = _entities.Get<Student>().Where(a => a.LoginIdentityID == aspNetUser.Id).SingleOrDefault();
			if (user == null)
			{
				var student = new Student(aspNetUser.Id);
				student.Email = aspNetUser.Email;
				_entities.Create(student);
				_entities.Save();
			}
		}

		public void CreateSupervisor(IdentityUser aspNetUser)
		{
			if (aspNetUser == null)
				throw new ArgumentNullException("aspNetUser");

			var now = DateTime.Now;
			var user = _entities.Get<Supervisor>().Where(a => a.LoginIdentityID == aspNetUser.Id).SingleOrDefault();
			if (user == null)
			{
				var supervisor = new Supervisor(aspNetUser.Id);
				supervisor.Email = aspNetUser.Email;
				supervisor.SupervisorType = SupervisorType.Principal;
				_entities.Create(supervisor);
				_entities.Save();
			}
		}

		public void CreateCoSupervisor(IdentityUser aspNetUser)
		{
			if (aspNetUser == null)
				throw new ArgumentNullException("aspNetUser");

			var now = DateTime.Now;
			var user = _entities.Get<CoSupervisor>().Where(a => a.LoginIdentityID == aspNetUser.Id).SingleOrDefault();
			if (user == null)
			{
				var coSupervisor = new CoSupervisor(aspNetUser.Id);
				coSupervisor.Email = aspNetUser.Email;
				coSupervisor.SupervisorType = SupervisorType.Coordinator;
				_entities.Create(coSupervisor);
				_entities.Save();
			}
		}

		public void CreateExternalSupervisor(IdentityUser aspNetUser)
		{
			if (aspNetUser == null)
				throw new ArgumentNullException("aspNetUser");

			var now = DateTime.Now;
			var user = _entities.Get<ExternalSupervisor>().Where(a => a.LoginIdentityID == aspNetUser.Id).SingleOrDefault();
			if (user == null)
			{
				var externalSupervisor = new ExternalSupervisor(aspNetUser.Id);
				externalSupervisor.Email = aspNetUser.Email;
				externalSupervisor.SupervisorType = SupervisorType.External;
				_entities.Create(externalSupervisor);
				_entities.Save();
			}
		}

		public void CreateCoordinator(IdentityUser aspNetUser)
		{
			if (aspNetUser == null)
				throw new ArgumentNullException("aspNetUser");

			var now = DateTime.Now;
			var user = _entities.Get<Coordinator>().Where(a => a.LoginIdentityID == aspNetUser.Id).SingleOrDefault();
			if (user == null)
			{
				var coordinator = new Coordinator(aspNetUser.Id);
				coordinator.Email = aspNetUser.Email;
				_entities.Create(coordinator);
				_entities.Save();
			}
		}

		public User FindUser(int? id)
		{
			return _entities.Get<User>(u => u.ID == id).SingleOrDefault();
		}

		public List<User> GetAllUser()
		{
			var users = _entities.Get<User>().ToList();
			return users;
		}

		public string GetUserRoleByID(int userID)
		{
			User user = FindUser(userID);
			return _userManager.GetRoles(user.LoginIdentityID).FirstOrDefault();
		}

		//User Email get user role
		public string GetCurrentRole(string loginIdentityName)
		{
			if (String.IsNullOrEmpty(loginIdentityName))
				return null;
			var user = _userManager.FindByName(loginIdentityName);
			if (user == null)
				return null;

			string role = "";
			if (user.Id != null)
				role = _userManager.GetRoles(user.Id).FirstOrDefault();

			if (role != null)
				return role;
			else
				return "";
		}

		public bool IsNewUser(string loginIdentityName)
		{
			var user = _userManager.FindByName(loginIdentityName);
			if (user == null)
				return false;

			string role = "";
			string firstName = "";
			if (user.Id != null)
				role = _userManager.GetRoles(user.Id).FirstOrDefault();

			if (role != null)
			{
				switch (role)
				{
					case "Student":
						var student = _entities.Get<Student>().Where(a => a.LoginIdentityID == user.Id).FirstOrDefault();
						if (student != null)
							firstName = student.Firstname;
						break;
					case "Supervisor":
						var supervisor = _entities.Get<Supervisor>().Where(a => a.LoginIdentityID == user.Id).FirstOrDefault();
						if (supervisor != null)
							firstName = supervisor.Firstname;
						break;
					case "CoSupervisor":
						var coSupervisor = _entities.Get<Supervisor>().Where(a => a.LoginIdentityID == user.Id).FirstOrDefault();
						if (coSupervisor != null)
							firstName = coSupervisor.Firstname;
						break;
					case "ExternalSupervisor":
						var extSupervisor = _entities.Get<Supervisor>().Where(a => a.LoginIdentityID == user.Id).FirstOrDefault();
						if (extSupervisor != null)
							firstName = extSupervisor.Firstname;
						break;
					case "Coordinator":
						var coordinator = _entities.Get<Coordinator>().Where(a => a.LoginIdentityID == user.Id).FirstOrDefault();
						if (coordinator != null)
							firstName = coordinator.Firstname;
						break;
				}
			}

			if (string.IsNullOrEmpty(firstName))
				return true;
			else
				return false;
		}

		public User GetUserIDByEmail(string loginIdentityName)
		{
			return _entities.Get<User>(u => u.Email == loginIdentityName).FirstOrDefault();
		}

		public int GetUserIDByIdentityID(string loginIdentityID)
		{
			var user = _entities.Get<User>(u => u.LoginIdentityID == loginIdentityID).FirstOrDefault();
			if (user != null)
				return user.ID;
			return 0;
		}

		#endregion

		#region Get current user

		public Student GetCurrentStudent(int userID)
		{
			return _entities.Get<Student>(c => c.ID == userID).SingleOrDefault();
		}

		public Supervisor GetCurrentSupervisor(int userID)
		{
			return _entities.Get<Supervisor>(c => c.ID == userID).SingleOrDefault();
		}

		public CoSupervisor GetCurrentCoSupervisor(int userID)
		{
			return _entities.Get<CoSupervisor>(c => c.ID == userID).SingleOrDefault();
		}

		public ExternalSupervisor GetCurrentExternalSupervisor(int userID)
		{
			return _entities.Get<ExternalSupervisor>(c => c.ID == userID).SingleOrDefault();
		}

		public Coordinator GetCurrentCoordinator(int userID)
		{
			return _entities.Get<Coordinator>(c => c.ID == userID).SingleOrDefault();
		}

		#endregion
	}
}