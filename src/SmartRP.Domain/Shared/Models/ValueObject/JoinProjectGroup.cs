using System;

namespace SmartRP.Domain
{
	public class JoinProjectGroup
	{

		#region Properties

		public int ID { set; get; }
		public int StudentID { get; set; }
		public int ProjectID { get; set; }
		public int TermID { get; set; }
		public int SubjectID { get; set; }
		public int GroupID { get; set; }
		public int ProposerID { get; set; }
		public RequestStatus RequestStatus { get; set; }
		public DateTime? UpdatedAt { get; set; }

		private bool CanBeRequested => RequestStatus == RequestStatus.None;
		private bool CanBeAccepted => RequestStatus == RequestStatus.Requested;
		private bool CanBeRejected => RequestStatus == RequestStatus.Requested;
		private bool CanBeWithdraw => RequestStatus == RequestStatus.Accepted;
		private bool CanBeRegistered => RequestStatus == RequestStatus.Accepted;
		private bool CanBeDeleted => RequestStatus != RequestStatus.Registered;
		private bool CanBeQuit => RequestStatus == RequestStatus.Registered;

		#endregion

		#region Ctor
		protected JoinProjectGroup()
		{
			// Required by EF
		}

		public JoinProjectGroup(int studentID, int projectID, int termID, int subjectID, int groupID, int proposerID)
		{
			StudentID = studentID;
			ProjectID = projectID;
			TermID = termID;
			SubjectID = subjectID;
			GroupID = groupID;
			ProposerID = proposerID;
			UpdatedAt = DateTime.Now;
			RequestStatus = RequestStatus.None;
		}

		#endregion

		#region Action

		public bool Request()
		{
			if (CanBeRequested)
			{
				RequestStatus = RequestStatus.Requested;
				UpdatedAt = DateTime.Now;
				return true;
			}
			return false;
		}

		public bool Accept()
		{
			if (CanBeAccepted)
			{
				RequestStatus = RequestStatus.Accepted;
				UpdatedAt = DateTime.Now;
				return true;
			}
			return false;
		}

		public bool Reject()
		{
			if (CanBeRejected)
			{
				RequestStatus = RequestStatus.Rejected;
				UpdatedAt = DateTime.Now;
				return true;
			}
			return false;
		}

		public bool Withdraw()
		{
			if (CanBeWithdraw)
			{
				RequestStatus = RequestStatus.Withdraw;
				UpdatedAt = DateTime.Now;
				return true;
			}
			return false;
		}

		public bool Register()
		{
			if (CanBeRegistered)
			{
				RequestStatus = RequestStatus.Registered;
				UpdatedAt = DateTime.Now;
				return true;
			}
			return false;
		}

		public bool Quit()
		{
			if (CanBeQuit)
			{
				RequestStatus = RequestStatus.Quit;
				UpdatedAt = DateTime.Now;
				return true;
			}
			return false;
		}

		public bool Delete()
		{
			if (CanBeDeleted)
			{
				RequestStatus = RequestStatus.Deleted;
				UpdatedAt = DateTime.Now;
				return true;
			}
			return false;
		}

		public void None()
		{
			RequestStatus = RequestStatus.None;
			UpdatedAt = DateTime.Now;
		}

		#endregion
	}
}
