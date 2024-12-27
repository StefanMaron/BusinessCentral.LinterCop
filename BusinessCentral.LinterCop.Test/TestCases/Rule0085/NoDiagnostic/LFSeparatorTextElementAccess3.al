codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyText: Text;
    begin
        [|MyText[3] := 10|];
    end;
}