codeunit 50000 MyCodeunit
{
    procedure MyProcedure(): Text[10]
    var
        MyText: Text[10];
        MyLabelLbl: Label 'My Label';
    begin
        MyText := [|MyLabelLbl|];
    end;
}