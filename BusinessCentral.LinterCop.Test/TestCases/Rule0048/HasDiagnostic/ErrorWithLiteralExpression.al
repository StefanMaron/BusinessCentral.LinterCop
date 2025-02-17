codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    begin
        [|Error('Something went wrong...')|];
    end;
}