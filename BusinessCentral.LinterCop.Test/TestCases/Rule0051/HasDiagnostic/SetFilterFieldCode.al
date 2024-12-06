codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
        MyFilterValue: Code[50];
    begin
        [|MyTable.SetFilter(MyField, '<>%1', MyFilterValue)|];
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Code[20]) { }
    }
}