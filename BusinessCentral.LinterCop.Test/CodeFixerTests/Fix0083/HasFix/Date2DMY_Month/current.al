codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyDate: Date;
        MonthValue: Integer;
    begin
        MonthValue := [|Date2DMY(MyDate, 2)|];
    end;
}
