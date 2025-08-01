// Report with no rendering section - should not trigger rule
report 50008 NoRenderingReport
{
    ProcessingOnly = true;

    dataset
    {
        dataitem(DataItemName; MyTable7)
        {
            column(ColumnName; MyField)
            {
            }
        }
    }
}

table 50006 MyTable7
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