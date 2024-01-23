using FluentMigrator;

namespace BeerDispenser.Infrastructure.Migrations;

[Migration(2)]
public class M0002_PaymentsAdded : Migration
{
    public override void Down()
    {
        Delete.Table("PaymentCard");
    }

    public override void Up()
    {
        Create.Table("PaymentCard")
            .WithColumn("Id").AsGuid().Indexed().NotNullable().PrimaryKey()
            .WithColumn("UserId").AsGuid().Indexed().NotNullable().PrimaryKey()
            .WithColumn("CardId").AsAnsiString().Indexed().NotNullable()
            .WithColumn("CustomerId").AsAnsiString().Indexed().NotNullable().PrimaryKey()
            .WithColumn("IsDefault").AsBoolean().NotNullable()
            .WithColumn("AdressCountry").AsAnsiString().Nullable()
            .WithColumn("Line1").AsAnsiString().Nullable()
            .WithColumn("State").AsAnsiString().Nullable()
            .WithColumn("Zip").AsAnsiString().Nullable()
            .WithColumn("Brand").AsAnsiString().Nullable()
            .WithColumn("City").AsAnsiString().Nullable()
            .WithColumn("Country").AsAnsiString().Nullable()
            .WithColumn("CvcCheck").AsAnsiString().Nullable()
            .WithColumn("Dynamiclast4").AsAnsiString().Nullable()
            .WithColumn("ExpMonth").AsAnsiString().Nullable()
            .WithColumn("ExpYear").AsAnsiString().Nullable()
            .WithColumn("Last4").AsAnsiString().Nullable()
            .WithColumn("AccountHolderName").AsAnsiString().Nullable()
            .WithColumn("ClientIp").AsAnsiString().Nullable()
            .WithColumn("Created").AsInt32().Nullable()
            .WithColumn("Email").AsAnsiString().Nullable();
    }
}

