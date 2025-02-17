codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyDateTime: DateTime;
        MyDate: Date;
    begin
        MyDate := [|DT2Date(MyDateTime)|];
    end;
}