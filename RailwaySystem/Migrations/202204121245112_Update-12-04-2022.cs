namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update12042022 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScheduledWayStations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WayStationId = c.Int(nullable: false),
                        ScheduleId = c.Int(nullable: false),
                        Arrival = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Departure = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Schedules", t => t.ScheduleId, cascadeDelete: false)
                .ForeignKey("dbo.WayStations", t => t.WayStationId, cascadeDelete: false)
                .Index(t => t.WayStationId)
                .Index(t => t.ScheduleId);
            
            AddColumn("dbo.Tracks", "StandardTicketPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SeatReservations", "Departure", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.SeatReservations", "Arrival", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.WayStations", "TicketPriceChange", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScheduledWayStations", "WayStationId", "dbo.WayStations");
            DropForeignKey("dbo.ScheduledWayStations", "ScheduleId", "dbo.Schedules");
            DropIndex("dbo.ScheduledWayStations", new[] { "ScheduleId" });
            DropIndex("dbo.ScheduledWayStations", new[] { "WayStationId" });
            DropColumn("dbo.WayStations", "TicketPriceChange");
            DropColumn("dbo.SeatReservations", "Arrival");
            DropColumn("dbo.SeatReservations", "Departure");
            DropColumn("dbo.Tracks", "StandardTicketPrice");
            DropTable("dbo.ScheduledWayStations");
        }
    }
}
