namespace ShoppingCartApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CartItems", "CartId", c => c.String());
            AddColumn("dbo.CartItems", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.CartItems", "ImagePath", c => c.String());
            DropColumn("dbo.CartItems", "Discount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CartItems", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.CartItems", "ImagePath");
            DropColumn("dbo.CartItems", "DateCreated");
            DropColumn("dbo.CartItems", "CartId");
        }
    }
}
