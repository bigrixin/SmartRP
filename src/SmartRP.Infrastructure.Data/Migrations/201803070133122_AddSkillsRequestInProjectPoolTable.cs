namespace SmartRP.Infrastructure.Data.Migrations
{
	using System.Data.Entity.Migrations;

	public partial class AddSkillsRequestInProjectPoolTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectPool", "SkillsRequest", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProjectPool", "SkillsRequest");
        }
    }
}
