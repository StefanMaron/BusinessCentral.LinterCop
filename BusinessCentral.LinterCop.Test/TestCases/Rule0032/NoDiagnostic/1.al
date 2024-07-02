table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer)
        {

        }
    }

    keys
    {
        key(Key1; MyField) { }
    }
}

codeunit 50100 MyCodeunit
{
    var
        MySingleInstanceCodeunitWithOnlyGlobalRecords: Codeunit MySingleInstanceCodeunit;

    procedure DoSomething()
    begin
        [|Clear(MySingleInstanceCodeunitWithOnlyGlobalRecords);|]
    end;
}

codeunit 50101 MySingleInstanceCodeunit
{
    SingleInstance = true;

    var
        GlobalRec: Record MyTable;
}