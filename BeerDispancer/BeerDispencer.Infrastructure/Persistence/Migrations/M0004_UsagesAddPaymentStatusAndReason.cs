using FluentMigrator;

namespace BeerDispenser.Infrastructure.Migrations;

[Migration(4)]
public class M0004_UsagesAddPaymentStatusAndReason : Migration
{
    public override void Down()
    {
        Alter.Table("Usage")
            .AlterColumn("Reason");
        Alter.Table("Usage").AlterColumn("PaymentStatus");
    }

    public override void Up()
    {
        Alter.Table("Usage")
           .AddColumn("PaymentStatus").AsInt32().Nullable();

        Alter.Table("Usage")
           .AddColumn("Reason").AsString().Nullable();
    }
}

