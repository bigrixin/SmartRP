namespace SmartRP.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSupervisorTypeInSupervisor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Supervisor", "SupervisorType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Supervisor", "SupervisorType");
        }
    }
}
