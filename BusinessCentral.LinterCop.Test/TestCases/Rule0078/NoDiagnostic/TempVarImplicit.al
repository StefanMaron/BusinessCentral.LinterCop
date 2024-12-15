codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable temporary;
    begin
        [|MyTable.MyField := 1|];
        [|MyTable.Insert()|];
        [|MyTable.Modify()|];
        [|MyTable.Delete()|];
        [|MyTable.DeleteAll()|];
        [|MyTable.ModifyAll(MyField, 1)|];
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer) { }
    }
}