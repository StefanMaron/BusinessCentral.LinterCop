codeunit 50100 MyCodeunit
{
    procedure MyProcedure(): Text[10]
    var
        MyLabelLbl: Label 'My Label';
    begin
        exit(Text.CopyStr(MyLabelLbl, 1, 10));
    end;
}