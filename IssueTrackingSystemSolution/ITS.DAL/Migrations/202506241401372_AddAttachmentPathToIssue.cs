namespace ITS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttachmentPathToIssue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Issues", "AttachmentPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Issues", "AttachmentPath");
        }
    }
}
