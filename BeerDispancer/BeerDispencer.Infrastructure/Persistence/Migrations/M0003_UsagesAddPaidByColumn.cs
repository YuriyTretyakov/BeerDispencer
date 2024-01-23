using FluentMigrator;

namespace BeerDispenser.Infrastructure.Migrations;

[Migration(3)]
public class M0003_UsagesAddPaidByColumn : Migration
{
    public override void Down()
    {
        Alter.Table("Usage")
            .AlterColumn("PaidBy");
    }

    public override void Up()
    {
        Alter.Table("Usage")
           .AddColumn("PaidBy").AsGuid().Nullable();
    }
}

