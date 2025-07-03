codeunit 50000 MyCodeunit
{
    procedure MyProcedure(): Text[10]
    var
        MyText: Text[10];
        MyLabelLbl: Label 'My Label longer than 10 characters';
    begin
        MyText := [|MyLabelLbl|];
    end;
}