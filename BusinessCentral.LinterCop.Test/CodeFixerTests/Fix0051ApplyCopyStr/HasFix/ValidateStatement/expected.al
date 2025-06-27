codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
        MyLabelLbl: Label 'My Label';
    begin
        MyTable.Validate(MyField, Text.CopyStr(MyLabelLbl, 1, Text.MaxStrLen(MyTable.MyField)));
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Code[10]) { }
    }
}