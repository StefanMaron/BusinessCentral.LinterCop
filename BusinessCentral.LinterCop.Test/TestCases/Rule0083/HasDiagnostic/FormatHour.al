codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTime: Time;
        i: Integer;
    begin
        Evaluate(i, [|Format(MyTime, 2, '<HOURS24>')|]);
    end;
}