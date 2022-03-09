namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update09032022 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScheduleTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SeatReservations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SeatId = c.Int(nullable: false),
                        ScheduleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Schedules", t => t.ScheduleId, cascadeDelete: false)
                .ForeignKey("dbo.Seats", t => t.SeatId, cascadeDelete: false)
                .Index(t => t.SeatId)
                .Index(t => t.ScheduleId);
            
            AddColumn("dbo.Schedules", "ScheduleTypeId", c => c.Int());
            AddColumn("dbo.Tickets", "Quantity", c => c.Int(nullable: false));
            AddColumn("dbo.Tickets", "SeatNumbers", c => c.String());
            CreateIndex("dbo.Schedules", "ScheduleTypeId");
            AddForeignKey("dbo.Schedules", "ScheduleTypeId", "dbo.ScheduleTypes", "Id");
            DropColumn("dbo.Seats", "Reserved");
            DropColumn("dbo.Tickets", "SeatNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tickets", "SeatNumber", c => c.Int(nullable: false));
            AddColumn("dbo.Seats", "Reserved", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.SeatReservations", "SeatId", "dbo.Seats");
            DropForeignKey("dbo.SeatReservations", "ScheduleId", "dbo.Schedules");
            DropForeignKey("dbo.Schedules", "ScheduleTypeId", "dbo.ScheduleTypes");
            DropIndex("dbo.SeatReservations", new[] { "ScheduleId" });
            DropIndex("dbo.SeatReservations", new[] { "SeatId" });
            DropIndex("dbo.Schedules", new[] { "ScheduleTypeId" });
            DropColumn("dbo.Tickets", "SeatNumbers");
            DropColumn("dbo.Tickets", "Quantity");
            DropColumn("dbo.Schedules", "ScheduleTypeId");
            DropTable("dbo.SeatReservations");
            DropTable("dbo.ScheduleTypes");
        }
    }
}
