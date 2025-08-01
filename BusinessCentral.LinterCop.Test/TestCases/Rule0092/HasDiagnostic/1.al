// Report with missing RDLC file in modern rendering section
report 50000 TestReport
{
    DefaultRenderingLayout = RDLCLayout;

    dataset
    {
        dataitem(DataItemName; MyTable)
        {
            column(ColumnName; MyField)
            {
            }
        }
    }

    rendering
    {
        layout(RDLCLayout)
        {
            Type = RDLC;
            [|LayoutFile = 'MissingReport.rdl'|];
            Caption = 'RDLC Layout';
        }
    }
}

table 50000 MyTable
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