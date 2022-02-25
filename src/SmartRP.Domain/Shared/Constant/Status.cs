
namespace SmartRP.Domain
{

	//student require joining a project group status
	public enum RequestStatus
	{
		Requested,   //student send request
		Rejected,    //publisher reject
		Accepted,    //publisher accept
		Withdraw,    //student withdraw request after publisher respond
		Registered,  //student confirm publisher accept
		Quit,        //student quit group before the group has a approved number
		Deleted,     //publisher deleted project before student confirm to register
		None         //no status
	}

	//project status
	public enum ProjectStatus
	{
		Pending,      //the project posted by student
		Opening,      //the project posted by supervisor
		Processing,   //the project posted by student that be picked up by a supervisor
		Full,         //the project has full, but do not have a supervisor
		Registered,   //the project has fulled and has a supervisor
		Completed,    //the project has finished
		Closed,        //the project has closed, do not allow student join
		Withdrawn     //the publisher has withdrawn the project
	}

	//pre project status
	public enum PreProjectStatus
	{
		Open,         //publisher post project open to all student
		Closed,       //Pre-Project register closed
		Completed,    //Pre-Project process finished
		Withdrawn     //publisher withdraw project
	}

	//group status
	public enum GroupStatus
	{
		Avaliable,    //the group has not full
		Full,         //the group has full
		Closed,       //the group close by supervisor, do not allow student join
		Marked        //the group get a mark
	}

}
