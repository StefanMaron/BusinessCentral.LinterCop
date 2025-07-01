codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyDate: Date;
        DayOfWeekValue: Integer;
    begin
        DayOfWeekValue := [|Date2DWY(MyDate, 1)|];
    end;
}
