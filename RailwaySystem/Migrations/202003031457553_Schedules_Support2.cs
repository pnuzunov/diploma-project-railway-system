namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Schedules_Support2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Schedules", "Departure", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Schedules", "Arrival", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Schedules", "Arrival");
            DropColumn("dbo.Schedules", "Departure");
        }
    }
}
