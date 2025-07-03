codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyDate: Date;
        WeekNoValue: Integer;
    begin
        WeekNoValue := MyDate.WeekNo();
    end;
}
