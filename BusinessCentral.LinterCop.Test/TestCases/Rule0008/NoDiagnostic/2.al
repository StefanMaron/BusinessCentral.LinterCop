codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        Field: Record Field;
    begin
        [|Field.SetRange(FieldName, 'Document No.')|];
    end;
}

table 50100 Field
{
    fields
    {
        field(1; TableNo; Integer) { }
        field(2; "No."; Integer) { }
        field(3; TableName; Text[30]) { }
        field(4; FieldName; Text[30]) { }
    }
}