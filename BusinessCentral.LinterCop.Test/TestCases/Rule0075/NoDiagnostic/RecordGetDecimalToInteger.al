codeunit 50100 MyCodeunit
{
    procedure MyProcedure(MyInteger: Integer)
    var
        MyTabe: Record MyTabe;
    begin
        [|MyTabe.Get(MyInteger)|];
    end;
}

table 50100 MyTabe
{
    fields
    {
        field(1; MyField; Decimal) { }
    }
}