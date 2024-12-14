codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        TempMyTable: Record MyTable temporary;
    begin
        if [|TempMyTable.Count() = 0|] then;
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer) { }
    }
}