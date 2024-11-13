namespace ShoppingCartApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddFileNameToProdct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ImageFileName", c => c.String());
            //DropColumn("dbo.Products", "ImagePath");
        }

        //    public override void Down()
        //    {
        //        AddColumn("dbo.Products", "ImagePath", c => c.String());
        //        DropColumn("dbo.Products", "ImageFileName");
        //    }
        //}
    }
}