codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
        MyLabelLbl: Label 'My Label', MaxLength = 10;
    begin
        MyTable.Validate(MyField, MyLabelLbl);
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Code[10]) { }
    }
}