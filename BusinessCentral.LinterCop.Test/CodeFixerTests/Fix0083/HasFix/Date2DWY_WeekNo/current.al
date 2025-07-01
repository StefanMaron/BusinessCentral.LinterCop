codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyDate: Date;
        WeekNoValue: Integer;
    begin
        WeekNoValue := [|Date2DWY(MyDate, 2)|];
    end;
}
