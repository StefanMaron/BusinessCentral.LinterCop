codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
    begin
        [|MyTable.Validate(MyField, 1)|];
        [|MyTable.Insert(true)|];
        [|MyTable.Modify(true)|];
        [|MyTable.Delete(true)|];
        [|MyTable.DeleteAll(true)|];
        [|MyTable.ModifyAll(MyField, 1, true)|];
    end;
}

table 50100 MyTable
{
    TableType = Temporary;

    fields
    {
        field(1; MyField; Integer) { }
    }
}