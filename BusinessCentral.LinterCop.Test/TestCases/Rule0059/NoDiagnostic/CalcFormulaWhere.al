table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer) { }
        field(2; MyFlowField; Integer)
        {
            FieldClass = FlowField;
            CalcFormula = count(MyTable where(MyField = filter([|'<>'''''|])));
        }
    }
}