namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Db_Update_03_02_2022 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CreditRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CustomerId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.EmployeeId, cascadeDelete: false)
                .Index(t => t.CustomerId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LevelOfAccess = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrainTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Seats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SeatTypeId = c.Int(nullable: false),
                        SeatNumber = c.Int(nullable: false),
                        Reserved = c.Boolean(nullable: false),
                        TrainId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SeatTypes", t => t.SeatTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Trains", t => t.TrainId, cascadeDelete: true)
                .Index(t => t.SeatTypeId)
                .Index(t => t.TrainId);
            
            CreateTable(
                "dbo.SeatTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        BeginStation = c.String(),
                        EndStation = c.String(),
                        Departure = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        BuyDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TrainName = c.String(),
                        TrainType = c.String(),
                        SeatNumber = c.Int(nullable: false),
                        QRCode = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Schedules", "PricePerTicket", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("dbo.Users", "RoleId");
            CreateIndex("dbo.Trains", "TypeId");
            AddForeignKey("dbo.Users", "RoleId", "dbo.UserRoles", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Trains", "TypeId", "dbo.TrainTypes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "CashAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Users", "Role", c => c.String());
            AddColumn("dbo.Trains", "Type", c => c.Short(nullable: false));
            DropForeignKey("dbo.Tickets", "UserId", "dbo.Users");
            DropForeignKey("dbo.Seats", "TrainId", "dbo.Trains");
            DropForeignKey("dbo.Seats", "SeatTypeId", "dbo.SeatTypes");
            DropForeignKey("dbo.Trains", "TypeId", "dbo.TrainTypes");
            DropForeignKey("dbo.CreditRecords", "EmployeeId", "dbo.Users");
            DropForeignKey("dbo.CreditRecords", "CustomerId", "dbo.Users");
            DropForeignKey("dbo.Users", "RoleId", "dbo.UserRoles");
            DropIndex("dbo.Tickets", new[] { "UserId" });
            DropIndex("dbo.Seats", new[] { "TrainId" });
            DropIndex("dbo.Seats", new[] { "SeatTypeId" });
            DropIndex("dbo.Trains", new[] { "TypeId" });
            DropIndex("dbo.Users", new[] { "RoleId" });
            DropIndex("dbo.CreditRecords", new[] { "EmployeeId" });
            DropIndex("dbo.CreditRecords", new[] { "CustomerId" });
            DropColumn("dbo.Users", "RoleId");
            DropColumn("dbo.Trains", "TypeId");
            DropColumn("dbo.Schedules", "PricePerTicket");
            DropTable("dbo.Tickets");
            DropTable("dbo.SeatTypes");
            DropTable("dbo.Seats");
            DropTable("dbo.TrainTypes");
            DropTable("dbo.UserRoles");
            DropTable("dbo.CreditRecords");
        }
    }
}
