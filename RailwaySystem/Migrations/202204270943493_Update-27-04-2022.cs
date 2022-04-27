namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update27042022 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PayPalPayments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TicketId = c.Int(nullable: false),
                        PaymentId = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .Index(t => t.TicketId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PayPalPayments", "TicketId", "dbo.Tickets");
            DropIndex("dbo.PayPalPayments", new[] { "TicketId" });
            DropTable("dbo.PayPalPayments");
        }
    }
}
