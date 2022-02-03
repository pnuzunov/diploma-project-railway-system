namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Stations_Support : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Stations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tracks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartStationId = c.Int(nullable: false),
                        EndStationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stations", t => t.EndStationId, cascadeDelete: false)
                .ForeignKey("dbo.Stations", t => t.StartStationId, cascadeDelete: false)
                .Index(t => t.StartStationId)
                .Index(t => t.EndStationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tracks", "StartStationId", "dbo.Stations");
            DropForeignKey("dbo.Tracks", "EndStationId", "dbo.Stations");
            DropIndex("dbo.Tracks", new[] { "EndStationId" });
            DropIndex("dbo.Tracks", new[] { "StartStationId" });
            DropTable("dbo.Tracks");
            DropTable("dbo.Stations");
        }
    }
}
