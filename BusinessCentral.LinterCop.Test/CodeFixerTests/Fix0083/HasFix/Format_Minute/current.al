codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTime: Time;
        MinuteValue: Integer;
    begin
        Evaluate(MinuteValue, [|Format(MyTime, 2, '<MINUTES>')|]);
    end;
}
