using System.ComponentModel.DataAnnotations.Schema;
using BeerDispencer.Infrastructure.Persistence.Entities;
using FluentMigrator;

namespace BeerDispencer.Infrastructure.Migrations;

[Migration(2)]
public class M0002_AddPaymentsAndOutbox : Migration
{
    public override void Down()
    {
        Delete.Table("Outbox");
        Delete.Table("Payments");
    }

    public override void Up()
    {
        Create.Table("Outbox")
             .WithColumn("Id").AsInt32().Indexed().NotNullable().Identity()
            .WithColumn("Data").AsString()
            .WithColumn("MessageType").AsString()
            .WithColumn("Status").AsInt16()
            .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable()
            .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").Nullable();


        Create.Table("Payments")
              .WithColumn("Id").AsInt32().Indexed().NotNullable().Identity()
            .WithColumn("DispencerId").AsGuid().ForeignKey("Dispencer", "Id")
            .WithColumn("Status").AsInt32().NotNullable()
            .WithColumn("Amount").AsDouble().NotNullable()
            .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable()
            .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").Nullable();

    }
}

