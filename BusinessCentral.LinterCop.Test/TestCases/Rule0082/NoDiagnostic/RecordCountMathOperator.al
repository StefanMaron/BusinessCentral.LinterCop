codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
        i: Integer;
    begin
        i := [|MyTable.Count() - 1|];
        i := [|MyTable.Count() + 1|];
        i := [|MyTable.Count() / 1|];
        i := [|MyTable.Count() * 1|];
        i := [|MyTable.Count() MOD 1|];
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; "Entry No."; Integer) { }
    }
}