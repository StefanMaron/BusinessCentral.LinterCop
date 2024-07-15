codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyCodeunit : Codeunit MyCodeunit;
    begin
        [|mycodeunit.Run();|]
    end;
}