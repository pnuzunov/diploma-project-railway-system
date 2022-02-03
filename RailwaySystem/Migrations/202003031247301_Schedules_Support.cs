namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Schedules_Support : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Schedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TrainId = c.Int(nullable: false),
                        TrackId = c.Int(nullable: false),
                        Departure = c.DateTime(nullable: false),
                        Arrival = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tracks", t => t.TrackId, cascadeDelete: true)
                .ForeignKey("dbo.Trains", t => t.TrainId, cascadeDelete: true)
                .Index(t => t.TrainId)
                .Index(t => t.TrackId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Schedules", "TrainId", "dbo.Trains");
            DropForeignKey("dbo.Schedules", "TrackId", "dbo.Tracks");
            DropIndex("dbo.Schedules", new[] { "TrackId" });
            DropIndex("dbo.Schedules", new[] { "TrainId" });
            DropTable("dbo.Schedules");
        }
    }
}
