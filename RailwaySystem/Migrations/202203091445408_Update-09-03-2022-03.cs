namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update0903202203 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SeatReservations", "Departure", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SeatReservations", "Departure");
        }
    }
}
