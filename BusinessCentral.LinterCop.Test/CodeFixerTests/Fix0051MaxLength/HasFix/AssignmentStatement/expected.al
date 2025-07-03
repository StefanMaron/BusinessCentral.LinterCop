codeunit 50100 MyCodeunit
{
    procedure MyProcedure(): Text[10]
    var
        MyText: Text[10];
        MyLabelLbl: Label 'My Label', MaxLength = 10;
    begin
        MyText := MyLabelLbl;
    end;
}