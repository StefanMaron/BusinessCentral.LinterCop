codeunit 50100 MyCodeunit
{
    procedure MyProcedure(): Text[10]
    var
        MyText: Text[10];
        MyLabelLbl: Label 'My Label';
    begin
        MyText := Text.CopyStr(MyLabelLbl, 1, Text.MaxStrLen(MyText));
    end;
}