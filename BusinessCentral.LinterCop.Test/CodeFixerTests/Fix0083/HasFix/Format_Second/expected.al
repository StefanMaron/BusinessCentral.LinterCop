codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTime: Time;
        SecondValue: Integer;
    begin
        Evaluate(SecondValue, MyTime.Second());
    end;
}
