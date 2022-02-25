namespace SmartRP.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeGradeWithNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Group", "Grade", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Group", "Grade", c => c.Int(nullable: false));
        }
    }
}
