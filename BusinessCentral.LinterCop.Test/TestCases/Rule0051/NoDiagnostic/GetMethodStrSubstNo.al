codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
        MyCodeFieldA, MyCodeFieldB : Code[10];
    begin
        MyTable.Get([|StrSubstNo('%1%2', MyCodeFieldA, MyCodeFieldB)|]);
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Code[20]) { }
    }
}