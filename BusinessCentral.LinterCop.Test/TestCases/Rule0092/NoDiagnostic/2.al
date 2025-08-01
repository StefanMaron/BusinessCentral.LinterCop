// Report extension with no layouts - should not trigger rule
report 50006 BaseReportForExtension
{
    ProcessingOnly = true;

    dataset
    {
        dataitem(DataItemName; MyTable6)
        {
            column(ColumnName; MyField)
            {
            }
        }
    }
}

reportextension 50007 EmptyReportExtension extends BaseReportForExtension
{
    dataset
    {
        add(DataItemName)
        {
            column(NewColumn; MyField)
            {
            }
        }
    }
}

table 50005 MyTable6
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