codeunit 50100 MemberAccessExpression
{
    procedure MyProcedure()
    begin
        Codeunit.Run([|codeunit|]::MyCodeunit);
    end;
}

codeunit 50101 MyCodeunit { }