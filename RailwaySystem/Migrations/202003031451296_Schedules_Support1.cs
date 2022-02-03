namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Schedules_Support1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Schedules", "Departure");
            DropColumn("dbo.Schedules", "Arrival");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Schedules", "Arrival", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Schedules", "Departure", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
    }
}
