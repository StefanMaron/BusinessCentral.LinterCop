codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
        MyGuid: Guid;
    begin
        if MyTable.[|GetBySystemId|](MyGuid) then;
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Code[20]) { }
    }
}