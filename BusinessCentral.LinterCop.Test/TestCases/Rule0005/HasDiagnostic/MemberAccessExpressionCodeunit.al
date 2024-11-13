codeunit 50100 OptionAccessExpression
{
    procedure MyProcedure()
    var
        i: Integer;
    begin
        i := [|codeunit|]::MyCodeunit;
    end;
}

codeunit 50101 MyCodeunit { }