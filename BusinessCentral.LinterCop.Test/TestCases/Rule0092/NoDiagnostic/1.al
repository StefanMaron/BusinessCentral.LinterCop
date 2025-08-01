// Report with no layout files defined - should not trigger rule
report 50005 NoLayoutReport
{
    ProcessingOnly = true;

    dataset
    {
        dataitem(DataItemName; MyTable5)
        {
            column(ColumnName; MyField)
            {
            }
        }
    }
}

table 50004 MyTable5
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