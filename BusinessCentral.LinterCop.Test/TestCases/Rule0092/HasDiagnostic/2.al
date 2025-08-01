// Report with missing layout file using legacy property
report 50001 LegacyReport
{
    [|RDLCLayout = 'MissingLegacyReport.rdl'|];

    dataset
    {
        dataitem(DataItemName; MyTable2)
        {
            column(ColumnName; MyField)
            {
            }
        }
    }
}

table 50001 MyTable2
{
    Caption = '', Locked = true;

    fields
    {
        field(1; MyField; Integer)
        {
            Caption = '', Locked = true;
            DataClassification = ToBeClassified;
        }
    }
}