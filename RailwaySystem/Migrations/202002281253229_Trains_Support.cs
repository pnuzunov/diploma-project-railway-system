namespace RailwaySystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Trains_Support : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.Trains",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.Short(nullable: false),
                    })
                    .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropTable("dbo.Trains");
        }
    }
}
