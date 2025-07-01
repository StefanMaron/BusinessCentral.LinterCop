codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyDateTime: DateTime;
        DateValue: Date;
    begin
        DateValue := [|DT2Date(MyDateTime)|];
    end;
}
