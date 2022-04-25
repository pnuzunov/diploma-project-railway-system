namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update2504222 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "ScheduleId", c => c.Int(nullable: false));
            AlterColumn("dbo.Schedules", "ScheduleModeId", c => c.Int(nullable: false));
            AlterColumn("dbo.Schedules", "Cancelled", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Tickets", "ScheduleId");
            AddForeignKey("dbo.Tickets", "ScheduleId", "dbo.Schedules", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "ScheduleId", "dbo.Schedules");
            DropIndex("dbo.Tickets", new[] { "ScheduleId" });
            AlterColumn("dbo.Schedules", "Cancelled", c => c.Boolean());
            AlterColumn("dbo.Schedules", "ScheduleModeId", c => c.Int());
            DropColumn("dbo.Tickets", "ScheduleId");
        }
    }
}
