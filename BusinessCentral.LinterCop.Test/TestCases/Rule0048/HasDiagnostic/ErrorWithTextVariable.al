codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        UnexpectedErr: Text;
    begin
        [|Error(UnexpectedErr)|];
    end;
}