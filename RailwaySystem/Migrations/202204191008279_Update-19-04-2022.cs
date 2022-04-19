namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update19042022 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tracks", "Description", c => c.String());
            DropColumn("dbo.Tracks", "StandardTicketPrice");
            DropColumn("dbo.WayStations", "TicketPriceChange");
            DropColumn("dbo.WayStations", "MinutesToArrive");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WayStations", "MinutesToArrive", c => c.Int(nullable: false));
            AddColumn("dbo.WayStations", "TicketPriceChange", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Tracks", "StandardTicketPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Tracks", "Description");
        }
    }
}
