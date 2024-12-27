codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyCode: Code[3];
    begin
        [|MyCode[3] := 10|];
    end;
}