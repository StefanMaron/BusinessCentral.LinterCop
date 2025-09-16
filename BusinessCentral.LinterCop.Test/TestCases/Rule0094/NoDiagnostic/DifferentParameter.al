table 50100 MyTable
{
    fields
    {
        field(1; Name; Text[100]) { }
    }

    procedure DoSth(var MyTableParam: Record MyTable)
    begin
    end;
}


codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
        MyTable2: Record MyTable;
    begin
        MyTable.DoSth([|MyTable2|]);
    end;
}