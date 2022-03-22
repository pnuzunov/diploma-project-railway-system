namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update220322 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CreditRecords", "EmployeeId", "dbo.Users");
            DropIndex("dbo.CreditRecords", new[] { "EmployeeId" });
            AlterColumn("dbo.CreditRecords", "EmployeeId", c => c.Int());
            CreateIndex("dbo.CreditRecords", "EmployeeId");
            AddForeignKey("dbo.CreditRecords", "EmployeeId", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CreditRecords", "EmployeeId", "dbo.Users");
            DropIndex("dbo.CreditRecords", new[] { "EmployeeId" });
            AlterColumn("dbo.CreditRecords", "EmployeeId", c => c.Int(nullable: false));
            CreateIndex("dbo.CreditRecords", "EmployeeId");
            AddForeignKey("dbo.CreditRecords", "EmployeeId", "dbo.Users", "Id", cascadeDelete: true);
        }
    }
}
