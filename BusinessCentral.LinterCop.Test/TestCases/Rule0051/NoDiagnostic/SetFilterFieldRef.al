codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
        SourceDocRef: RecordRef;
    begin
        [|MyTable.SetFilter(MyField, '<>%1', SourceDocRef.Field(MyTable.FieldNo(MyField)).Value)|];
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Code[20]) { }
    }
}