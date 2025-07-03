codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyDateTime: DateTime;
        TimeValue: Time;
    begin
        TimeValue := MyDateTime.Time();
    end;
}
