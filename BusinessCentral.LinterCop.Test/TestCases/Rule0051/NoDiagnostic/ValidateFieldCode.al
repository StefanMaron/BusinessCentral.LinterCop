codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
        MyVar: Code[10];
    begin
        MyTable.Validate(MyField, [|MyVar|]);
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Code[10]) { }
    }
}