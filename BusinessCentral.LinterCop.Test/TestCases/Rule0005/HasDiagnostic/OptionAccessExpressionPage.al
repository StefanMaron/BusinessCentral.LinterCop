codeunit 50100 OptionAccessExpression
{
    procedure MyProcedure()
    begin
        Page.Run([|page|]::MyPage);
    end;
}

page 50100 MyPage { }