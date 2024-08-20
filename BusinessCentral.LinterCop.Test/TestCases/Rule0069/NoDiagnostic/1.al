codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTableRecord: Record MyTable;
    begin
        if MyTableRecord.FindFirst() then[|;|] // empty statement used with if to avoid runtime error if find fails
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Code[20]) { }
    }

    keys
    {
        key(Key1; MyField)
        {
            Clustered = true;
        }
    }
}