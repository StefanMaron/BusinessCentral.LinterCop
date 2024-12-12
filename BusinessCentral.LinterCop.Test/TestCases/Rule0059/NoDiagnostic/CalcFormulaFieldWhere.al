table 50100 MyTable
{
    fields
    {
        field(1; "Primary Key"; Code[10]) { }
        field(2; MyFlowField; Code[20])
        {
            FieldClass = FlowField;
            CalcFormula = lookup(SomeTable.MyField where(MyField = filter([|'<>'''''|])));
        }

    }
}

table 50101 SomeTable
{
    fields
    {
        field(1; MyField; Code[20]) { }
    }
}