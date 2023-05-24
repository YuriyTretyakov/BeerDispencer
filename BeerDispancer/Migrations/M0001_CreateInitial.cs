using System.ComponentModel.DataAnnotations.Schema;
using FluentMigrator;

namespace Migrations;

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
            .WithColumn("Id").AsGuid().Indexed().NotNullable().WithDefault(SystemMethods.NewSequentialId).PrimaryKey()
            .WithColumn("Volume").AsDouble()
            .WithColumn("Status").AsInt16();


        Create.Table("Usage")
            .WithColumn("Id").AsInt32().Identity().PrimaryKey()
            .WithColumn("DispencerId").AsGuid().ForeignKey("Dispencer", "Id")
            .WithColumn("OpenAt").AsCustom("timestamp with time zone").NotNullable()
            .WithColumn("ClosedAt").AsCustom("timestamp with time zone").Nullable()
            .WithColumn("FlowVolume").AsDouble().Nullable()
            .WithColumn("TotalSpent").AsDouble().Nullable();


    }
}

