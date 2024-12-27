codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyText: Text;
    begin
        [|MyText[1] := 10|];
    end;
}