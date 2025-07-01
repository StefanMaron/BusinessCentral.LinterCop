codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTime: Time;
        MillisecondValue: Integer;
    begin
        Evaluate(MillisecondValue, MyTime.Millisecond());
    end;
}
