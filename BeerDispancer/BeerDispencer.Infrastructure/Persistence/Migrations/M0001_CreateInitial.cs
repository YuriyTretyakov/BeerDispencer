using System.ComponentModel.DataAnnotations.Schema;
using FluentMigrator;

namespace BeerDispenser.Infrastructure.Migrations;

[Migration(1)]
public class M0001_CreateInitial : Migration
{
    public override void Down()
    {
        Delete.Table("Dispencer");
        Delete.Table("Usage");
    }

    public override void Up()
    {
        Create.Table("Dispencer")
            .WithColumn("Id").AsGuid().Indexed().NotNullable().PrimaryKey()
            .WithColumn("Volume").AsDecimal()
            .WithColumn("Status").AsInt32()
            .WithColumn("ReservedFor").AsAnsiString().Nullable()
            .WithColumn("IsActive").AsBoolean().NotNullable();



        Create.Table("Usage")
            .WithColumn("Id").AsGuid().Indexed().NotNullable().PrimaryKey()
            .WithColumn("DispencerId").AsGuid().ForeignKey("Dispencer", "Id")
            .WithColumn("OpenAt").AsDateTimeOffset().NotNullable()
            .WithColumn("ClosedAt").AsDateTimeOffset().Nullable()
            .WithColumn("FlowVolume").AsDecimal().Nullable()
            .WithColumn("TotalSpent").AsDecimal().Nullable();


    }
}

