namespace SmartRP.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ADDMaxGroupNumberInProject : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Project", "MaxGroupNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Project", "MaxGroupNumber");
        }
    }
}
