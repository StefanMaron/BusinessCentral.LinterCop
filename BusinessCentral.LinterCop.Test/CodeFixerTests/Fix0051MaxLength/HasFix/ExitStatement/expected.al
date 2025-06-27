codeunit 50100 MyCodeunit
{
    procedure MyProcedure(): Text[10]
    var
        MyLabelLbl: Label 'My Label', MaxLength = 10;
    begin
        exit(MyLabelLbl);
    end;
}