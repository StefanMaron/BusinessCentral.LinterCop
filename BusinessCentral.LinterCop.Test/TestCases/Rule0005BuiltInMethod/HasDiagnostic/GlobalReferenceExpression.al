codeunit 50100 MyCodeunit
{
    var
        "My Table": Record "My Table";

    procedure MyProcedure()
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

    var
        "My Table": Record "My Table";

    procedure MyProcedure()
    begin
        [|"MY TABLE"|].Get();
    end;
}