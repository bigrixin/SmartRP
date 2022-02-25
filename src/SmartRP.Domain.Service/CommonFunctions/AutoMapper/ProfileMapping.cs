using AutoMapper;

namespace SmartRP.Domain.Service.AutoMapper
{
	public class ProfileMapping : Profile
	{

		#region Properties

		private readonly ICommonService _commonServices;

		#endregion

		#region Action

		public ProfileMapping(ICommonService commonServices)
		{
			_commonServices = commonServices;
			UserProfileMappers();
			TermMappers();
			KeywordMappers();
			ProjectMappers();
			GroupMappers();
		}

		#endregion

		#region Mappers

		private void UserProfileMappers()
		{

			CreateMap<Student, CurrentTermStudentViewModel>()
			 .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.LoginIdentityID))
			 	.ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "Student"))
				//.ForMember(dest => dest.EnrolledSubjects, opt => opt.MapFrom(src => src.GetEnrolledSubjects()))
				.ForMember(dest => dest.EnrolledSubjects, opt => opt.Ignore())
				.ForMember(dest => dest.PostedProjects, opt => opt.MapFrom(src => src.GetProjects()))
			 	.ForMember(dest => dest.JoinedGroups, opt => opt.MapFrom(src => src.GetEnrolledGroups()))
				 .ForMember(dest => dest.RequestedProjectGroups, opt => opt.MapFrom(src => src.GetRequestedProjectGroups()))
				.ForMember(dest => dest.UserSelectedKeywords, opt => opt.Ignore());
			//.ForMember(dest => dest.UserSelectedKeywords, opt => opt.MapFrom(src => src.GetUserKeywords()));

			//coordinater
			CreateMap<Coordinator, CoordinatorViewModel>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.LoginIdentityID))
				.ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "Coordinator"));

			; CreateMap<Coordinator, CoordinatorProfileEditModel>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.LoginIdentityID))
				.ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "Coordinator"));

			CreateMap<CoordinatorProfileEditModel, Coordinator>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));

			//supervisor
			CreateMap<Supervisor, SupervisorViewModel>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.LoginIdentityID))
				.ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "Supervisor"));

			CreateMap<Supervisor, SupervisorProfileEditModel>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "Supervisor"))
				 .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.LoginIdentityID));

			CreateMap<SupervisorProfileEditModel, Supervisor>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));

			//co-supervisor
			CreateMap<CoSupervisor, CoSupervisorViewModel>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.LoginIdentityID))
				.ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "CoSupervisor"))
				.ForMember(dest => dest.EnrolledSubjects, opt => opt.Ignore());

			CreateMap<CoSupervisor, CoSupervisorProfileEditModel>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.LoginIdentityID))
				.ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "CoSupervisor"));

			CreateMap<CoSupervisorProfileEditModel, CoSupervisor>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));

			//external-supervisor
			CreateMap<ExternalSupervisor, ExternalSupervisorViewModel>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.LoginIdentityID))
				.ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "ExternalSupervisor"))
				.ForMember(dest => dest.EnrolledSubjects, opt => opt.Ignore());

			CreateMap<ExternalSupervisor, ExternalSupervisorProfileEditModel>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.LoginIdentityID))
				.ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "ExternalSupervisor"));

			CreateMap<ExternalSupervisorProfileEditModel, ExternalSupervisor>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));

			//student
			CreateMap<Student, StudentViewModel>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.LoginIdentityID))
			 	.ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "Student"))
				.ForMember(dest => dest.EnrolledSubjects, opt => opt.Ignore());

			CreateMap<Student, StudentProfileEditModel>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.LoginIdentityID))
			 	.ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "Student"));

			CreateMap<StudentProfileEditModel, Student>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));
		}

		private void TermMappers()
		{
			//term
			CreateMap<Term, TermViewModel>();
			CreateMap<TermViewModel, Term>()
			 .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
			 .ForMember(dest => dest.Session, opt => opt.MapFrom(src => src.Session));

			CreateMap<Term, TermEditModel>()
			 .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
			 .ForMember(dest => dest.Session, opt => opt.MapFrom(src => src.Session))
			 .ForMember(dest => dest.CheckBoxSubjectNames, opt => opt.MapFrom(src => _commonServices.GetInitialSubjectNames(src.ID)));

			CreateMap<TermEditModel, Term>()
			 .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
			 .ForMember(dest => dest.TermName, opt => opt.MapFrom(src => src.ID != 0 ? src.TermName : src.Year + src.Session.ToString()))
			 .ForMember(dest => dest.StartAt, opt => opt.MapFrom(src => src.ID != 0 ? src.StartAt : _commonServices.GetTermStartTime(src.Session, src.Year)))
			 .ForMember(dest => dest.EndAt, opt => opt.MapFrom(src => src.ID != 0 ? src.EndAt : _commonServices.GetTermEndTime(src.Session, src.Year)));

			CreateMap<Term, EnrolSubjectViewModel>()
			 .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
			 .ForMember(dest => dest.StartAt, opt => opt.MapFrom(src => src.StartAt))
			 .ForMember(dest => dest.SubjectNameDropDownList, opt => opt.MapFrom(src => _commonServices.GetTermSubjectSelectList(src.ID)));

			CreateMap<EnrolSubjectViewModel, Term>();
		}

		private void KeywordMappers()
		{
			//keyword
			CreateMap<Keyword, KeywordViewModel>()
			 .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
			 .ForMember(dest => dest.CoordinatorID, opt => opt.MapFrom(src => src.CoordinatorID));
			CreateMap<KeywordViewModel, Keyword>();
		}

		private void ProjectMappers()
		{
			CreateMap<ProjectPool, ProjectPoolEditModel>()
			 .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));
			CreateMap<ProjectPoolEditModel, ProjectPool>();

			CreateMap<ProjectPool, Project>()
			 .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
			 .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.SupervisorID))
			 .ForMember(dest => dest.ProjectPoolID, opt => opt.MapFrom(src => src.ID))
			 .ForMember(dest => dest.PublisherType, opt => opt.Ignore())
			 .ForMember(dest => dest.Status, opt => opt.Ignore());

			CreateMap<Project, ProjectViewModel>()
			 .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
			 .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => _commonServices.GetSubjectNameByID(src.SubjectID)))
			 .ForMember(dest => dest.SemesterName, opt => opt.MapFrom(src => _commonServices.GetSemesterBySubjectID(src.SubjectID).TermName));

			CreateMap<ProjectViewModel, Project>();

			CreateMap<Project, ProjectEditModel>()
			 .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));

			CreateMap<ProjectEditModel, Project>()
			 .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
			 .ForMember(dest => dest.PublisherType, opt => opt.MapFrom(src => src.PublisherType));

			CreateMap<CoSupervisor, MyCoSupervisorViewModel>()
		 .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
		 .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Firstname + " " + src.Lastname))
		 .ForMember(dest => dest.ProjectPoolCounter, opt => opt.MapFrom(src => src.GetProjectPools().Count))
		 .ForMember(dest => dest.PublishedProjectCounter, opt => opt.MapFrom(src => src.GetProjects().Count));
		}

		private void GroupMappers()
		{
			CreateMap<JoinProjectGroup, RequestToJoinGroupViewModel>()
			 .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));

			CreateMap<Report, ReportViewModel>()
			 .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));
			CreateMap<ReportViewModel, Report>()
			 .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));

			CreateMap<Group, GroupViewModel>()
			 .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));

			CreateMap<Group, ReportCommentViewModel>()
			.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
			.ForMember(dest => dest.JoinedStudents, opt => opt.MapFrom(src => src.GetStudents()))
			.ForMember(dest => dest.PostedReports, opt => opt.MapFrom(src => src.GetReports()));

			CreateMap<Group, GradeGroupViewModel>()
			.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
			.ForMember(dest => dest.JoinedStudents, opt => opt.MapFrom(src => src.GetStudents()))
			.ForMember(dest => dest.PostedReports, opt => opt.MapFrom(src => src.GetReports()));
		}

		#endregion

	}
}
