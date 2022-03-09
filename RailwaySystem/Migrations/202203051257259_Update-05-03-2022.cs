namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update05032022 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Seats", "SeatTypeId", "dbo.SeatTypes");
            DropIndex("dbo.Seats", new[] { "SeatTypeId" });
            AddColumn("dbo.Seats", "IsFirstClass", c => c.Boolean(nullable: false));
            DropColumn("dbo.Seats", "SeatTypeId");
            DropTable("dbo.SeatTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SeatTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Seats", "SeatTypeId", c => c.Int(nullable: false));
            DropColumn("dbo.Seats", "IsFirstClass");
            CreateIndex("dbo.Seats", "SeatTypeId");
            AddForeignKey("dbo.Seats", "SeatTypeId", "dbo.SeatTypes", "Id", cascadeDelete: true);
        }
    }
}
