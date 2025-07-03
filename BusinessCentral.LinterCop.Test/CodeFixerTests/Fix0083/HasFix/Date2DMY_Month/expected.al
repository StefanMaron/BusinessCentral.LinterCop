codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyDate: Date;
        MonthValue: Integer;
    begin
        MonthValue := MyDate.Month();
    end;
}
