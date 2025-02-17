codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
        i: Integer;
        b: Boolean;
        t: Time;
    begin
        [|Today|];
        [|WorkDate|];
        [|GuiAllowed|];
        t := [|Time|];
        i := MyTable.[|Count|];
        b := MyTable.[|IsEmpty|];
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer) { }
    }
}