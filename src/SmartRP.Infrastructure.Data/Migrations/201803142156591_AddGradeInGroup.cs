namespace SmartRP.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGradeInGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Group", "Grade", c => c.Int(nullable: false));
            AddColumn("dbo.Group", "Comments", c => c.String());
            AddColumn("dbo.Group", "CommentDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Group", "CommentDate");
            DropColumn("dbo.Group", "Comments");
            DropColumn("dbo.Group", "Grade");
        }
    }
}
