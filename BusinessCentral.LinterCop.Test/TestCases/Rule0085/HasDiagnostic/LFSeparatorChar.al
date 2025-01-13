codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        myChar: Char;
    begin
        [|myChar := 10|];
    end;
}