codeunit 50000 MyCodeunit
{
    procedure MyProcedure(): Text[10]
    var
        MyLabelLbl: Label 'My Label';
    begin
        exit([|MyLabelLbl|]);
end;
}