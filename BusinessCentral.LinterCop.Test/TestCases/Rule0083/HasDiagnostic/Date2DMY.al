codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyDate: Date;
        MyInteger: Integer;
    begin
        MyInteger := [|Date2DMY(MyDate, 1)|];
    end;
}