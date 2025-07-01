codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTime: Time;
        HourValue: Integer;
    begin
        Evaluate(HourValue, [|Format(MyTime, 2, '<HOURS24>')|]);
    end;
}
