codeunit 50100 OptionAccessExpression
{
    procedure MyProcedure()
    var
        i: Integer;
    begin
        i := [|page|]::MyPage;
    end;
}

page 50100 MyPage { }