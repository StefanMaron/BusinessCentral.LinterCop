codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
    begin
        MyTable.Get([|Database.UserId()|]);
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; UserName; Code[50]) { }
    }
}