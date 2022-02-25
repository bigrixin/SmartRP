******************************************
 !!! CURRENT USED VERSION !!! - 20190716
******************************************
 https://smartrp.azurewebsites.net/
=========================================================================
This is Helen Lu's Account (haiyan.lu@uts.edu.au)

https://portal.azure.com
steven_zhai@hotmail.com  (guest)
 >R.2018

<add name="SmartRPDbContext" 
	connectionString="Server=tcp:rputsadmin.database.windows.net,1433;Initial Catalog=smartrp202;
  Persist Security Info=False;User ID=rputs2017db;Password=rpUTS2017;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" 
	providerName="System.Data.SqlClient" />
	
	
 SQL Server:
	Server: tcp:rputsadmin.database.windows.net
	Login: rputs2017db
	
usr:$smartrp
ftp://waws-prod-sy3-021.ftp.azurewebsites.windows.net
S5GHb0A0uNpszPE1ovgE9Ev2LB9rbM3y8YwvLFf0crkgpmbx0fdgX2NEwnbe
=========================================================================
Research Project Management Portal (SmartRP2)
                       - V2.021 March 2018
v20180604

=========================================================================
CS TFS
11305644@student.uts.edu.au  
https://thefirstteam.visualstudio.com



bigrixin@hotmail.com   7113
smartrp.visualstudio.com

publish
https://smartrp.azurewebsites.net/

=========================================================================
 New features
=========================================================================
    -Include Elmas diagnosis
    -Migration for update table
    -Fix login bugs
    -Multiple group of a project increase automatically
    -Keyword matching
    -New roles

=========================================================================
 A new structure and some rules will be created for version 2:
=========================================================================
    1. A Student can enrol many subjects, but only can enrol one subject during a term (OK)
    2. A Student can only join one project group in a subject (OK)
    3. Add two roles which are Co-supervisor and External-supervisor (OK)
    4. Co-supervisor and External-supervisor can enrol many subjects in one term (OK)
    5. Coordinator and Supervisor can cross any subjects at any terms and subjects (OK)

=========================================================================
 Tutorials for Student:
=========================================================================
    A1. Registration and fill the profile. 
     a. New registration.(An verify Email will be received)
     b. Fill profile.
     c. Enroll a subject. (Each semester should enroll a new subject). 
     d. Update the profile
    https://youtu.be/80j4o5arEeQ

    A2. Send requests and confirm an offer of joining a project group.
     a. Select an interesting project.
     b. Send a request to join a project group.
     c. Wait for the response to my request. (An Email will be received)
     e. Confirm an offer to join a project group. 
    https://youtu.be/5EqZCd13gEI
	 
    A3. Manage the project group
    - The Leader is the first student who joins the group.
     a. Withdraw from a group. (Before the group is full)
     b. View the group status.
     c. Change the group information. (Leader only)
     d. Upload the report. (Leader only when the group is full)
    https://youtu.be/bLsTn-dI-ks
 
    A4. Post a project  (optional)
     a. Post a project.
     b. Find a suitable supervisor. (keyword-based matching) 
         - Invite selected supervisors to supervise the project.
     c. Find fellow  students (keyword-based matching) 
         - Invite them to join the project group. 
     d. Manage the requests from the students.
         - Accept or Reject a request.  (An Email will be received)
         - Change the group information. (group name and description,group leader only)
         - Upload the reports. (When the group is full and a supervisor secured)
         - Delete the project. (Only for the cases that no other students joined the group)
    https://youtu.be/ewOaeGS-y7I

=========================================================================
 Tutorials for Supervisor:
=========================================================================
    B1. Registration and fill the profile. 
     a. New registration.(An verify Email will be received)
     b. Fill profile.
     d. Update the profile
    https://youtu.be/gz7wDv5KwB0

    B2. Post a project  
     a. Post a project into my project pool. (Can be updated or deleted anytime)
     b. Publish a project from my project pool. (Published ones are separate copies and can be updated) 
     c. Find fellow students. (Keyword-based matching) 
         - Invite the students to join the project group. (Optional)
     d. Delete a project group. (Only for the cases that no other students joined the group) 
    https://youtu.be/cvwbNXsg15o

    B3. Manage project group
     a. View the group status.
     b. Wait for the requests from the students. (An Email will be received)
     c. Manage the requests.
         - Accept or Reject a request.  (An Email will be received)
         - Comments the report.(When the group is full and there are reports uploaded)
    https://youtu.be/RHcokUBkmok

    B4. Pick up projects
     a. Pick up the projects that posted by students
     b. Manage the project group
    https://youtu.be/YQ4IFI7yvys

=========================================================================
 Tutorials for Co-Supervisor 
=========================================================================
    C1. Registration and fill the profile. (An verify Email will be received)
     a. New registration.
     b. Fill profile.
	 d. Enroll a subject
     e. Update the profile
    https://youtu.be/8l-21QeUCGE

    C2. Post a project
     a. Post a project into my project pool. (Can be updated or deleted anytime)
     b. Publish a project from my project pool. (Published ones are separate copies and can be updated) 
     c. Find fellow students. (Keyword-based matching) 
         - Invite the students to join the project group. (Optional)
     d. Delete a project group. (Only for the cases that no other students joined the group)
    https://youtu.be/cvwbNXsg15o

    C3. Manage project group
     a. View the group status.
     b. Wait for the requests from the students. (An Email will be received)
     c. Manage the requests.
         - Accept or Reject a request.  (An Email will be received)
         - Comments the report.(When the group is full and there are reports uploaded)
    https://youtu.be/RHcokUBkmok

    C4. Pick up projects
     a. Pick up the projects that posted by students
     b. Manage the project group
    https://youtu.be/YQ4IFI7yvys

=========================================================================
 Tutorials for Ext-Supervisor
=========================================================================
    D1. Registration and fill the profile. (An verify Email will be received)
     a. New registration.
     b. Fill profile.
	 d. Enroll a subject
     e. Update the profile
    https://youtu.be/8l-21QeUCGE

    D2. Post a project
     a. Post a project into my project pool. (Can be updated or deleted anytime)
     b. Publish a project from my project pool. (Published ones are separate copies and can be updated) 
     c. Find fellow students. (Keyword-based matching) 
         - Invite the students to join the project group. (Optional)
     d. Delete a project group. (Only for the cases that no other students joined the group)
    https://youtu.be/Jn_Ay2uvjlY

    D3. Manage project group
     a. View the group status.
     b. Wait for the requests from the students. (An Email will be received)
     c. Manage the requests.
         - Accept or Reject a request.  (An Email will be received)
         - Comments the report.(When the group is full and there are reports uploaded)
    https://youtu.be/dNSlJVG8TDg

=========================================================================
 Q&A:
=========================================================================
    Q: Error message: This page isn¡¯t working
	smartrp.azurewebsites.net redirected you too many times.
	Try clearing your cookies.
	ERR_TOO_MANY_REDIRECTS

    A: Clearing the cookies of  Chrome web browser
	 https://youtu.be/9A52t2PreTA

    Q: Error message: The page isn¡¯t redirecting properly 
	Firefox has detected that the server is redirecting the request for this address in a way that will never complete. 
	This problem can sometimes be caused by disabling or refusing to accept cookies."

    A: Clearing the cookies of Firefox web browser
	https://youtu.be/KziwnHRwyls

    Q: Forgot password
    A: Reset password
	https://youtu.be/rNVxHCbdqBA


