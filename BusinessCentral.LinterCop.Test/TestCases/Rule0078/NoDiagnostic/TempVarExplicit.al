codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable temporary;
    begin
        [|MyTable.MyField := 1|];
        [|MyTable.Insert(false)|];
        [|MyTable.Modify(false)|];
        [|MyTable.Delete(false)|];
        [|MyTable.DeleteAll(false)|];
        [|MyTable.ModifyAll(MyField, 1, false)|];
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer) { }
    }
}