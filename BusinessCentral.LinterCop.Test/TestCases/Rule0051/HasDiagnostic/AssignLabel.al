codeunit 50000 MyCodeunit
{
    internal procedure MyProcedure(): Text[10]
    var
        MyText: Text[10];
        MyLabelLbl: Label 'My Label';
    begin
        MyText := [|MyLabelLbl|];
    end;
}