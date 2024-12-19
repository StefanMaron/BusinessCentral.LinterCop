codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyDateTime: DateTime;
        MyTime: Time;
    begin
        MyTime := [|DT2Time(MyDateTime)|];
    end;
}