codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTime: Time;
        HourValue: Integer;
    begin
        Evaluate(HourValue, MyTime.Hour());
    end;
}
