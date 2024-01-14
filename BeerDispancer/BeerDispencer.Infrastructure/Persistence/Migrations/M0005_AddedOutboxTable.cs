using FluentMigrator;

namespace BeerDispenser.Infrastructure.Migrations;

[Migration(5)]
public class M0005_AddedOutboxTable : Migration
{
    public override void Down()
    {
        Delete.Table("Outbox");
    }

    public override void Up()
    {
        Create.Table("Outbox")
            .WithColumn("Id").AsGuid().Indexed().NotNullable().PrimaryKey()
            .WithColumn("EventType").AsAnsiString().NotNullable()
            .WithColumn("Payload").AsAnsiString().NotNullable()
            .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable()
            .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").Nullable()
            .WithColumn("EventState").AsInt16().NotNullable();
    }
}
