codeunit 50000 MyCodeunit
{
    internal procedure MyProcedure(): Text[10]
    var
        MyLabelLbl: Label 'My Label longer than 10 characters';
    begin
        exit([|MyLabelLbl|]);
end;
}