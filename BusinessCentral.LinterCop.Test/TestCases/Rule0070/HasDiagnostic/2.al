codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        myList: List of [Integer];
        i: Integer;
    begin
        for i := [|0|] to myList.Count() do;
    end;
}