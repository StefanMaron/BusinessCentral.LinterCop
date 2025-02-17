codeunit 50100 MyCodeunit
{
    procedure MyProcedure(MyCode: Code[10])
    var
        MyTabe: Record MyTabe;
    begin
        [|MyTabe.Get(MyCode)|];
    end;
}

table 50100 MyTabe
{
    fields
    {
        field(1; MyField; Code[20]) { }
    }
}