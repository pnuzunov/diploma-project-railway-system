namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update250422 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "PaymentMethod", c => c.Int(nullable: false));
            AddColumn("dbo.Schedules", "Cancelled", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Schedules", "Cancelled");
            DropColumn("dbo.Tickets", "PaymentMethod");
        }
    }
}
