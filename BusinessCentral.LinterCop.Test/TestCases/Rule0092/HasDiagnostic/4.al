// Report with multiple missing layout files
report 50004 MultipleLayoutsReport
{
    DefaultRenderingLayout = RDLCLayout;

    dataset
    {
        dataitem(DataItemName; MyTable4)
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
            [|LayoutFile = 'MissingReport1.rdl'|];
            Caption = 'RDLC Layout';
        }

        layout(WordLayout)
        {
            Type = Word;
            [|LayoutFile = 'MissingDocument.docx'|];
            Caption = 'Word Layout';
        }

        layout(ExcelLayout)
        {
            Type = Excel;
            [|LayoutFile = 'MissingSpreadsheet.xlsx'|];
            Caption = 'Excel Layout';
        }
    }
}

table 50003 MyTable4
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