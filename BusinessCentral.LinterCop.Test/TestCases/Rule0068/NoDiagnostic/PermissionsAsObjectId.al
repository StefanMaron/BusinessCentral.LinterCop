codeunit 50000 MyCodeunit
{
    Permissions = tabledata 50000 = r;

    trigger OnRun()
    var
        MyTable: Record MyTable;
    begin
        [|MyTable.FindFirst();|]
    end;
}

table 50000 MyTable
{
    fields
    {
        field(1; MyField; Integer) { }
    }
}