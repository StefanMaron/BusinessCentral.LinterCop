codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
    begin
        MyTable.SetRange([|"My Filter"|], '1');
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer) { }
        field(2; "My Filter"; Code[10])
        {
            FieldClass = FlowFilter;
        }
    }
}