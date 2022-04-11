namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update11042022 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WayStations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TrackId = c.Int(nullable: false),
                        StationId = c.Int(nullable: false),
                        ConsecutiveNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stations", t => t.StationId, cascadeDelete: true)
                .ForeignKey("dbo.Tracks", t => t.TrackId, cascadeDelete: true)
                .Index(t => t.TrackId)
                .Index(t => t.StationId);
            
            AddColumn("dbo.CreditRecords", "TicketId", c => c.Int());
            AddColumn("dbo.Stations", "CityId", c => c.Int(nullable: false));
            CreateIndex("dbo.CreditRecords", "TicketId");
            CreateIndex("dbo.Stations", "CityId");
            AddForeignKey("dbo.CreditRecords", "TicketId", "dbo.Tickets", "Id");
            AddForeignKey("dbo.Stations", "CityId", "dbo.Cities", "Id", cascadeDelete: true);
            DropColumn("dbo.SeatReservations", "Departure");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SeatReservations", "Departure", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            DropForeignKey("dbo.WayStations", "TrackId", "dbo.Tracks");
            DropForeignKey("dbo.WayStations", "StationId", "dbo.Stations");
            DropForeignKey("dbo.Stations", "CityId", "dbo.Cities");
            DropForeignKey("dbo.CreditRecords", "TicketId", "dbo.Tickets");
            DropIndex("dbo.WayStations", new[] { "StationId" });
            DropIndex("dbo.WayStations", new[] { "TrackId" });
            DropIndex("dbo.Stations", new[] { "CityId" });
            DropIndex("dbo.CreditRecords", new[] { "TicketId" });
            DropColumn("dbo.Stations", "CityId");
            DropColumn("dbo.CreditRecords", "TicketId");
            DropTable("dbo.WayStations");
            DropTable("dbo.Cities");
        }
    }
}
