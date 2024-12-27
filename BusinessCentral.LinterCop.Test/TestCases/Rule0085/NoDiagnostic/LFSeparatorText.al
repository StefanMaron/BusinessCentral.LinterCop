codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyText: Text;
    begin
        [|MyText[2] := 10|];
    end;
}