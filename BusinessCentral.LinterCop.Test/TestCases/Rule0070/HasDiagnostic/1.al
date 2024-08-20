codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        myList: List of [Integer];
        i: Integer;
    begin
        i := myList.Get([|0|]);
    end;
}