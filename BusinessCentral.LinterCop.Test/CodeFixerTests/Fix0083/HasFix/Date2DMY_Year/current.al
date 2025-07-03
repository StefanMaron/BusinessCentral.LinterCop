codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyDate: Date;
        YearValue: Integer;
    begin
        YearValue := [|Date2DMY(MyDate, 3)|];
    end;
}
