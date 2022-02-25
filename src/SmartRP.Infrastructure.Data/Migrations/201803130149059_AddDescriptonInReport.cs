namespace SmartRP.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDescriptonInReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Report", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Report", "Description");
        }
    }
}
