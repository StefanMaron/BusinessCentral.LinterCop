// Report extension with missing layout file
report 50002 BaseReport
{
    DefaultRenderingLayout = RDLCLayout;

    dataset
    {
        dataitem(DataItemName; MyTable3)
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
            LayoutFile = 'ExistingLayout.rdl';
            Caption = 'Base RDLC Layout';
        }
    }
}

reportextension 50003 TestReportExtension extends BaseReport
{
    rendering
    {
        layout(ExcelLayout)
        {
            Type = Excel;
            [|LayoutFile = 'MissingExcelLayout.xlsx'|];
            Caption = 'Excel Layout';
        }
    }
}

table 50002 MyTable3
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