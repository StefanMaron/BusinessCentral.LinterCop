codeunit 50100 MemberAccessExpression
{
    procedure MyProcedure()
    begin
        Codeunit.Run([|database|]::MyTable);
    end;
}

table 50100 MyTable { fields { field(1; Myfield; Code[10]) { } } }