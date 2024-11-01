codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTableA: Record MyTableA;
        MyTableB: Record MyTableB;
    begin
        [|MyTableA.TransferFields(MyTableB)|];
    end;
}

table 50100 MyTableA
{
    fields
    {
        field(1; "No."; Code[20]) { }
    }
}
table 50101 MyTableB
{
    fields
    {
        field(1; "No."; Code[20]) { }
    }
}
