namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update11042022_StationsLatLng : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stations", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Stations", "Longitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stations", "Longitude");
            DropColumn("dbo.Stations", "Latitude");
        }
    }
}
