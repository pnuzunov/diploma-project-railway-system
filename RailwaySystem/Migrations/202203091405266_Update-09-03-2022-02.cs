namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update0903202202 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScheduleCancellations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ScheduleId = c.Int(nullable: false),
                        CancellationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Schedules", t => t.ScheduleId, cascadeDelete: true)
                .Index(t => t.ScheduleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScheduleCancellations", "ScheduleId", "dbo.Schedules");
            DropIndex("dbo.ScheduleCancellations", new[] { "ScheduleId" });
            DropTable("dbo.ScheduleCancellations");
        }
    }
}
