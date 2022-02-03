namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Email", c => c.String());
            AddColumn("dbo.Users", "Phone", c => c.String());
            AddColumn("dbo.Users", "Role", c => c.String());
            AddColumn("dbo.Users", "CashAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "CashAmount");
            DropColumn("dbo.Users", "Role");
            DropColumn("dbo.Users", "Phone");
            DropColumn("dbo.Users", "Email");
        }
    }
}
