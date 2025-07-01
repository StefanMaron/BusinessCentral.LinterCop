codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyDate: Date;
        DayValue: Integer;
    begin
        DayValue := [|Date2DMY(MyDate, 1)|];
    end;
}
