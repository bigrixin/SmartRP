namespace SmartRP.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.User",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Firstname = c.String(),
                        Lastname = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        Introduction = c.String(),
                        ResumeURL = c.String(),
                        SuggestedKeyword = c.String(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        LoginIdentityID = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Keyword",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        CoordinatorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Coordinator", t => t.CoordinatorID)
                .Index(t => t.CoordinatorID);
            
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        DocumentURL = c.String(),
                        SkillsRequest = c.String(),
                        Status = c.Int(nullable: false),
                        ApprovedNumber = c.String(),
                        GroupSize = c.Int(nullable: false),
                        ExpiredAt = c.DateTime(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        PublisherType = c.String(),
                        UserID = c.Int(nullable: false),
                        SubjectID = c.Int(nullable: false),
                        ProjectPoolID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                //.ForeignKey("dbo.ProjectPool", t => t.ProjectPoolID)
                .ForeignKey("dbo.Subject", t => t.SubjectID, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.SubjectID)
                .Index(t => t.ProjectPoolID);
            
            CreateTable(
                "dbo.Group",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        Size = c.Int(nullable: false),
                        Vacancy = c.Int(nullable: false),
                        LeaderID = c.Int(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        SupervisorID = c.Int(),
                        CoSupervisorID = c.Int(),
                        ExtSupervisorID = c.Int(),
                        ProjectID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Project", t => t.ProjectID, cascadeDelete: true)
                .ForeignKey("dbo.Supervisor", t => t.SupervisorID)
                .Index(t => t.SupervisorID)
                .Index(t => t.ProjectID);
            
            CreateTable(
                "dbo.Report",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ReportType = c.Int(nullable: false),
                        FileURL = c.String(),
                        Comments = c.String(),
                        Grade = c.Int(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        GroupID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Group", t => t.GroupID, cascadeDelete: true)
                .Index(t => t.GroupID);
            
            CreateTable(
                "dbo.JoinProjectGroup",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StudentID = c.Int(nullable: false),
                        ProjectID = c.Int(nullable: false),
                        TermID = c.Int(nullable: false),
                        SubjectID = c.Int(nullable: false),
                        GroupID = c.Int(nullable: false),
                        ProposerID = c.Int(nullable: false),
                        RequestStatus = c.Int(nullable: false),
                        UpdatedAt = c.DateTime(),
                        Supervisor_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Student", t => t.StudentID)
                .ForeignKey("dbo.Supervisor", t => t.Supervisor_ID)
                .Index(t => t.StudentID)
                .Index(t => t.Supervisor_ID);
            
            CreateTable(
                "dbo.Subject",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SubjectName = c.Int(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        TermID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Term", t => t.TermID, cascadeDelete: true)
                .Index(t => t.TermID);
            
            CreateTable(
                "dbo.ProjectKeyword",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ProjectID = c.Int(nullable: false),
                        KeywordID = c.Int(nullable: false),
                        ProjectPool_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ProjectPool", t => t.ProjectPool_ID)
                .ForeignKey("dbo.Project", t => t.ProjectID, cascadeDelete: true)
                .Index(t => t.ProjectID)
                .Index(t => t.ProjectPool_ID);
            
            CreateTable(
                "dbo.Reference",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Authors = c.String(),
                        Citation = c.String(),
                        ReferenceURL = c.String(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        ProjectID = c.Int(nullable: false),
                        ProjectPool_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ProjectPool", t => t.ProjectPool_ID)
                .ForeignKey("dbo.Project", t => t.ProjectID, cascadeDelete: true)
                .Index(t => t.ProjectID)
                .Index(t => t.ProjectPool_ID);
            
            CreateTable(
                "dbo.Skill",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        CoordinatorID = c.Int(nullable: false),
                        ProjectPool_ID = c.Int(),
                        Project_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ProjectPool", t => t.ProjectPool_ID)
                .ForeignKey("dbo.Project", t => t.Project_ID)
                .ForeignKey("dbo.Coordinator", t => t.CoordinatorID)
                .Index(t => t.CoordinatorID)
                .Index(t => t.ProjectPool_ID)
                .Index(t => t.Project_ID);
            
            CreateTable(
                "dbo.TRPToRP",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NewProjectID = c.Int(nullable: false),
                        Comments = c.String(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        ProjectID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Project", t => t.ProjectID, cascadeDelete: true)
                .Index(t => t.ProjectID);
            
            CreateTable(
                "dbo.ProjectPool",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        DocumentURL = c.String(),
                        GroupSize = c.Int(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        SupervisorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Supervisor", t => t.SupervisorID)
                .Index(t => t.SupervisorID);
            
            CreateTable(
                "dbo.UserKeyword",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        KeywordID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Term",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TermName = c.String(),
                        Session = c.Int(nullable: false),
                        StartAt = c.DateTime(),
                        EndAt = c.DateTime(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        CoordinatorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Coordinator", t => t.CoordinatorID)
                .Index(t => t.CoordinatorID);
            
            CreateTable(
                "dbo.ProjectSkill",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ProjectID = c.Int(nullable: false),
                        SkillID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.UserSkill",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        SkillID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.StudentGroup",
                c => new
                    {
                        Student_ID = c.Int(nullable: false),
                        Group_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Student_ID, t.Group_ID })
                .ForeignKey("dbo.Student", t => t.Student_ID, cascadeDelete: true)
                .ForeignKey("dbo.Group", t => t.Group_ID, cascadeDelete: true)
                .Index(t => t.Student_ID)
                .Index(t => t.Group_ID);
            
            CreateTable(
                "dbo.CoSupervisorSubject",
                c => new
                    {
                        CoSupervisor_ID = c.Int(nullable: false),
                        Subject_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CoSupervisor_ID, t.Subject_ID })
                .ForeignKey("dbo.CoSupervisor", t => t.CoSupervisor_ID, cascadeDelete: true)
                .ForeignKey("dbo.Subject", t => t.Subject_ID, cascadeDelete: true)
                .Index(t => t.CoSupervisor_ID)
                .Index(t => t.Subject_ID);
            
            CreateTable(
                "dbo.ExternalSupervisorSubject",
                c => new
                    {
                        ExternalSupervisor_ID = c.Int(nullable: false),
                        Subject_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ExternalSupervisor_ID, t.Subject_ID })
                .ForeignKey("dbo.ExternalSupervisor", t => t.ExternalSupervisor_ID, cascadeDelete: true)
                .ForeignKey("dbo.Subject", t => t.Subject_ID, cascadeDelete: true)
                .Index(t => t.ExternalSupervisor_ID)
                .Index(t => t.Subject_ID);
            
            CreateTable(
                "dbo.SubjectStudent",
                c => new
                    {
                        Subject_ID = c.Int(nullable: false),
                        Student_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Subject_ID, t.Student_ID })
                .ForeignKey("dbo.Subject", t => t.Subject_ID, cascadeDelete: true)
                .ForeignKey("dbo.Student", t => t.Student_ID, cascadeDelete: true)
                .Index(t => t.Subject_ID)
                .Index(t => t.Student_ID);
            
            CreateTable(
                "dbo.Coordinator",
                c => new
                    {
                        ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.Supervisor",
                c => new
                    {
                        ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.CoSupervisor",
                c => new
                    {
                        ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Supervisor", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.ExternalSupervisor",
                c => new
                    {
                        ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Supervisor", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.PreProject",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        PreProjectStatus = c.Int(nullable: false),
                        SupervisorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Project", t => t.ID)
                .ForeignKey("dbo.Supervisor", t => t.SupervisorID)
                .Index(t => t.ID)
                .Index(t => t.SupervisorID);
            
            CreateTable(
                "dbo.Student",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        StudentID = c.String(),
                        Degree = c.String(),
                        PhotoURL = c.String(),
                        GPA = c.Single(),
                        HasJoinedCurrentSubjectProjectGroup = c.Boolean(nullable: false),
                        PreProjectID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.ID)
                .ForeignKey("dbo.PreProject", t => t.PreProjectID)
                .Index(t => t.ID)
                .Index(t => t.PreProjectID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Student", "PreProjectID", "dbo.PreProject");
            DropForeignKey("dbo.Student", "ID", "dbo.User");
            DropForeignKey("dbo.PreProject", "SupervisorID", "dbo.Supervisor");
            DropForeignKey("dbo.PreProject", "ID", "dbo.Project");
            DropForeignKey("dbo.ExternalSupervisor", "ID", "dbo.Supervisor");
            DropForeignKey("dbo.CoSupervisor", "ID", "dbo.Supervisor");
            DropForeignKey("dbo.Supervisor", "ID", "dbo.User");
            DropForeignKey("dbo.Coordinator", "ID", "dbo.User");
            DropForeignKey("dbo.UserKeyword", "UserID", "dbo.User");
            DropForeignKey("dbo.Project", "UserID", "dbo.User");
            DropForeignKey("dbo.JoinProjectGroup", "Supervisor_ID", "dbo.Supervisor");
            DropForeignKey("dbo.ProjectPool", "SupervisorID", "dbo.Supervisor");
            DropForeignKey("dbo.Group", "SupervisorID", "dbo.Supervisor");
            DropForeignKey("dbo.Term", "CoordinatorID", "dbo.Coordinator");
            DropForeignKey("dbo.Subject", "TermID", "dbo.Term");
            DropForeignKey("dbo.Skill", "CoordinatorID", "dbo.Coordinator");
            DropForeignKey("dbo.TRPToRP", "ProjectID", "dbo.Project");
            DropForeignKey("dbo.Skill", "Project_ID", "dbo.Project");
            DropForeignKey("dbo.Reference", "ProjectID", "dbo.Project");
            DropForeignKey("dbo.ProjectKeyword", "ProjectID", "dbo.Project");
            DropForeignKey("dbo.Group", "ProjectID", "dbo.Project");
            DropForeignKey("dbo.SubjectStudent", "Student_ID", "dbo.Student");
            DropForeignKey("dbo.SubjectStudent", "Subject_ID", "dbo.Subject");
            DropForeignKey("dbo.Project", "SubjectID", "dbo.Subject");
            DropForeignKey("dbo.ExternalSupervisorSubject", "Subject_ID", "dbo.Subject");
            DropForeignKey("dbo.ExternalSupervisorSubject", "ExternalSupervisor_ID", "dbo.ExternalSupervisor");
            DropForeignKey("dbo.CoSupervisorSubject", "Subject_ID", "dbo.Subject");
            DropForeignKey("dbo.CoSupervisorSubject", "CoSupervisor_ID", "dbo.CoSupervisor");
            DropForeignKey("dbo.Skill", "ProjectPool_ID", "dbo.ProjectPool");
            DropForeignKey("dbo.Reference", "ProjectPool_ID", "dbo.ProjectPool");
            DropForeignKey("dbo.Project", "ProjectPoolID", "dbo.ProjectPool");
            DropForeignKey("dbo.ProjectKeyword", "ProjectPool_ID", "dbo.ProjectPool");
            DropForeignKey("dbo.JoinProjectGroup", "StudentID", "dbo.Student");
            DropForeignKey("dbo.StudentGroup", "Group_ID", "dbo.Group");
            DropForeignKey("dbo.StudentGroup", "Student_ID", "dbo.Student");
            DropForeignKey("dbo.Report", "GroupID", "dbo.Group");
            DropForeignKey("dbo.Keyword", "CoordinatorID", "dbo.Coordinator");
            DropIndex("dbo.Student", new[] { "PreProjectID" });
            DropIndex("dbo.Student", new[] { "ID" });
            DropIndex("dbo.PreProject", new[] { "SupervisorID" });
            DropIndex("dbo.PreProject", new[] { "ID" });
            DropIndex("dbo.ExternalSupervisor", new[] { "ID" });
            DropIndex("dbo.CoSupervisor", new[] { "ID" });
            DropIndex("dbo.Supervisor", new[] { "ID" });
            DropIndex("dbo.Coordinator", new[] { "ID" });
            DropIndex("dbo.SubjectStudent", new[] { "Student_ID" });
            DropIndex("dbo.SubjectStudent", new[] { "Subject_ID" });
            DropIndex("dbo.ExternalSupervisorSubject", new[] { "Subject_ID" });
            DropIndex("dbo.ExternalSupervisorSubject", new[] { "ExternalSupervisor_ID" });
            DropIndex("dbo.CoSupervisorSubject", new[] { "Subject_ID" });
            DropIndex("dbo.CoSupervisorSubject", new[] { "CoSupervisor_ID" });
            DropIndex("dbo.StudentGroup", new[] { "Group_ID" });
            DropIndex("dbo.StudentGroup", new[] { "Student_ID" });
            DropIndex("dbo.Term", new[] { "CoordinatorID" });
            DropIndex("dbo.UserKeyword", new[] { "UserID" });
            DropIndex("dbo.ProjectPool", new[] { "SupervisorID" });
            DropIndex("dbo.TRPToRP", new[] { "ProjectID" });
            DropIndex("dbo.Skill", new[] { "Project_ID" });
            DropIndex("dbo.Skill", new[] { "ProjectPool_ID" });
            DropIndex("dbo.Skill", new[] { "CoordinatorID" });
            DropIndex("dbo.Reference", new[] { "ProjectPool_ID" });
            DropIndex("dbo.Reference", new[] { "ProjectID" });
            DropIndex("dbo.ProjectKeyword", new[] { "ProjectPool_ID" });
            DropIndex("dbo.ProjectKeyword", new[] { "ProjectID" });
            DropIndex("dbo.Subject", new[] { "TermID" });
            DropIndex("dbo.JoinProjectGroup", new[] { "Supervisor_ID" });
            DropIndex("dbo.JoinProjectGroup", new[] { "StudentID" });
            DropIndex("dbo.Report", new[] { "GroupID" });
            DropIndex("dbo.Group", new[] { "ProjectID" });
            DropIndex("dbo.Group", new[] { "SupervisorID" });
            DropIndex("dbo.Project", new[] { "ProjectPoolID" });
            DropIndex("dbo.Project", new[] { "SubjectID" });
            DropIndex("dbo.Project", new[] { "UserID" });
            DropIndex("dbo.Keyword", new[] { "CoordinatorID" });
            DropTable("dbo.Student");
            DropTable("dbo.PreProject");
            DropTable("dbo.ExternalSupervisor");
            DropTable("dbo.CoSupervisor");
            DropTable("dbo.Supervisor");
            DropTable("dbo.Coordinator");
            DropTable("dbo.SubjectStudent");
            DropTable("dbo.ExternalSupervisorSubject");
            DropTable("dbo.CoSupervisorSubject");
            DropTable("dbo.StudentGroup");
            DropTable("dbo.UserSkill");
            DropTable("dbo.ProjectSkill");
            DropTable("dbo.Term");
            DropTable("dbo.UserKeyword");
            DropTable("dbo.ProjectPool");
            DropTable("dbo.TRPToRP");
            DropTable("dbo.Skill");
            DropTable("dbo.Reference");
            DropTable("dbo.ProjectKeyword");
            DropTable("dbo.Subject");
            DropTable("dbo.JoinProjectGroup");
            DropTable("dbo.Report");
            DropTable("dbo.Group");
            DropTable("dbo.Project");
            DropTable("dbo.Keyword");
            DropTable("dbo.User");
        }
    }
}
