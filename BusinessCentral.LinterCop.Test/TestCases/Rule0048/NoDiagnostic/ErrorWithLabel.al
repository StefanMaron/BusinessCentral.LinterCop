codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        UnexpectedErr: Label 'Something went wrong...';
    begin
        [|Error(UnexpectedErr)|];
    end;
}