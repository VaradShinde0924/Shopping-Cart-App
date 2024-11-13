namespace ShoppingCartApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIssueTypeToContactMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContactMessages", "IssueType", c => c.String(nullable: false));
            AddColumn("dbo.ContactMessages", "SubmittedAt", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ContactMessages", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.ContactMessages", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.ContactMessages", "Message", c => c.String(nullable: false));
            DropColumn("dbo.ContactMessages", "HelpCategory");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ContactMessages", "HelpCategory", c => c.String(nullable: false));
            AlterColumn("dbo.ContactMessages", "Message", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.ContactMessages", "LastName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.ContactMessages", "FirstName", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.ContactMessages", "SubmittedAt");
            DropColumn("dbo.ContactMessages", "IssueType");
        }
    }
}
