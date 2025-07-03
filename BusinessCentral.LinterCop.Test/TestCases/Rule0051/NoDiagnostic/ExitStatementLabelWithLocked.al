codeunit 50000 MyCodeunit
{
    procedure MyProcedure(): Text[10]
    var
        MyLabelLbl: Label 'My Label', Locked = true;
    begin
        exit([|MyLabelLbl|]);
end;
}