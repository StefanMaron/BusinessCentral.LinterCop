codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
    begin
        MyTable.Name := [|'Standard'|];
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; Name; Text[100]) { }
    }
}