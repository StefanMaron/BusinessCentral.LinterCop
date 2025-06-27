codeunit 50100 MyCodeunit
{
    procedure MyProcedure(): Text[10]
    var
        MyLabelLbl: Label 'My Label';
    begin
        exit([|MyLabelLbl|]);
    end;
}