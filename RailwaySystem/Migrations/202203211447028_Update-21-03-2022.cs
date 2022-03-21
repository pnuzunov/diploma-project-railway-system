namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update21032022 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SeatReservations", "TicketId", c => c.Int(nullable: false));
            CreateIndex("dbo.SeatReservations", "TicketId");
            AddForeignKey("dbo.SeatReservations", "TicketId", "dbo.Tickets", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SeatReservations", "TicketId", "dbo.Tickets");
            DropIndex("dbo.SeatReservations", new[] { "TicketId" });
            DropColumn("dbo.SeatReservations", "TicketId");
        }
    }
}
