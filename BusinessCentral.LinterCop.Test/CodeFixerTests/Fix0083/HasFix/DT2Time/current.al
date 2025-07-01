codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyDateTime: DateTime;
        TimeValue: Time;
    begin
        TimeValue := [|DT2Time(MyDateTime)|];
    end;
}
