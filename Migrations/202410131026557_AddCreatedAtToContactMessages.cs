namespace ShoppingCartApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCreatedAtToContactMessages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContactMessages", "CreatedAt", c => c.DateTime());
            DropColumn("dbo.ContactMessages", "SubmittedAt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ContactMessages", "SubmittedAt", c => c.DateTime(nullable: false));
            DropColumn("dbo.ContactMessages", "CreatedAt");
        }
    }
}
