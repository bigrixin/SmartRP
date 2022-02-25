namespace SmartRP.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSupervisorIDInCo_Ext_Supervisor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CoSupervisor", "MySupervisorID", c => c.Int(nullable: false));
            AddColumn("dbo.ExternalSupervisor", "MySupervisorID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExternalSupervisor", "MySupervisorID");
            DropColumn("dbo.CoSupervisor", "MySupervisorID");
        }
    }
}
