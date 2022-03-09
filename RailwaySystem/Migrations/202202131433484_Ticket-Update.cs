namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TicketUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "SeatType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "SeatType");
        }
    }
}
