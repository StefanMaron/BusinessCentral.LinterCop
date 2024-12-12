codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
        i: Integer;
        b: Boolean;
        d: Date;
    begin
        d := [|Today()|];
        d := [|WorkDate()|];
        b := [|GuiAllowed()|];
        i := MyTable.[|Count()|];
        b := MyTable.[|IsEmpty()|];
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer) { }
    }
}