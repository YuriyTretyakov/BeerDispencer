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
            .WithColumn("Payload").AsAnsiString(2000).NotNullable()
            .WithColumn("CreatedAt").AsDateTimeOffset().NotNullable()
            .WithColumn("UpdatedAt").AsDateTimeOffset().Nullable()
            .WithColumn("EventState").AsInt32().NotNullable();
    }
}
