namespace SmartRP.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApprovedNOInGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Group", "ApprovedNO", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Group", "ApprovedNO");
        }
    }
}
