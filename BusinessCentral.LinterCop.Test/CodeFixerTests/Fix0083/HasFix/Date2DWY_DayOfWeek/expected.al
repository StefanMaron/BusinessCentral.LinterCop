codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyDate: Date;
        DayOfWeekValue: Integer;
    begin
        DayOfWeekValue := MyDate.DayOfWeek();
    end;
}
