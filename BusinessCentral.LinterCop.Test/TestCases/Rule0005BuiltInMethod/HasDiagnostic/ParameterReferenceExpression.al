codeunit 50100 MyCodeunit
{
    procedure MyProcedure("My Table": Record "My Table")
    begin
        [|"MY TABLE"|].Get();
    end;
}

table 50100 "My Table"
{
    fields
    {
        field(1; MyField; Integer) { }
    }

    procedure MyProcedure("My Table": Record "My Table")
    begin
        [|"MY TABLE"|].Get();
    end;
}