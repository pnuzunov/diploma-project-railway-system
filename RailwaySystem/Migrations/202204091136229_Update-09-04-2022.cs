namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update09042022 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Schedules", "ScheduleTypeId", "dbo.ScheduleTypes");
            DropForeignKey("dbo.ScheduleCancellations", "ScheduleId", "dbo.Schedules");
            DropIndex("dbo.ScheduleCancellations", new[] { "ScheduleId" });
            DropIndex("dbo.Schedules", new[] { "ScheduleTypeId" });
            AddColumn("dbo.Users", "ClientNumber", c => c.String());
            AddColumn("dbo.Schedules", "ScheduleModeId", c => c.Int());
            AddColumn("dbo.Tickets", "Arrival", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            DropColumn("dbo.Schedules", "ScheduleTypeId");
            DropTable("dbo.ScheduleCancellations");
            DropTable("dbo.ScheduleTypes");
        }
        
        public override void Down()
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
                "dbo.ScheduleCancellations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ScheduleId = c.Int(nullable: false),
                        CancellationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Schedules", "ScheduleTypeId", c => c.Int());
            DropColumn("dbo.Tickets", "Arrival");
            DropColumn("dbo.Schedules", "ScheduleModeId");
            DropColumn("dbo.Users", "ClientNumber");
            CreateIndex("dbo.Schedules", "ScheduleTypeId");
            CreateIndex("dbo.ScheduleCancellations", "ScheduleId");
            AddForeignKey("dbo.ScheduleCancellations", "ScheduleId", "dbo.Schedules", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Schedules", "ScheduleTypeId", "dbo.ScheduleTypes", "Id");
        }
    }
}
