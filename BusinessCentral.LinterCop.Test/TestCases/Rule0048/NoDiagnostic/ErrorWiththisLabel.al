codeunit 50100 MyCodeunit
{
    var
        UnexpectedErr: Label 'Something went wrong...';

    procedure MyProcedure()
    begin
        [|Error(this.UnexpectedErr)|];
    end;
}