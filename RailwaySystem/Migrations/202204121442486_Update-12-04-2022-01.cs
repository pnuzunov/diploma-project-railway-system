namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update1204202201 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tracks", "EndStationId", "dbo.Stations");
            DropForeignKey("dbo.Tracks", "StartStationId", "dbo.Stations");
            DropIndex("dbo.Tracks", new[] { "StartStationId" });
            DropIndex("dbo.Tracks", new[] { "EndStationId" });
            AddColumn("dbo.WayStations", "MinutesToArrive", c => c.Int(nullable: false));
            DropColumn("dbo.Tracks", "StartStationId");
            DropColumn("dbo.Tracks", "EndStationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tracks", "EndStationId", c => c.Int(nullable: false));
            AddColumn("dbo.Tracks", "StartStationId", c => c.Int(nullable: false));
            DropColumn("dbo.WayStations", "MinutesToArrive");
            CreateIndex("dbo.Tracks", "EndStationId");
            CreateIndex("dbo.Tracks", "StartStationId");
            AddForeignKey("dbo.Tracks", "StartStationId", "dbo.Stations", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tracks", "EndStationId", "dbo.Stations", "Id", cascadeDelete: true);
        }
    }
}
