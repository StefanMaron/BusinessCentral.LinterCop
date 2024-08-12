report 50000 MyReport
{
    ProcessingOnly = true;

    dataset
    {
        [|dataitem(DataItemName; MyTable)|]
        {
            column(ColumnName; MyField)
            {

            }
        }
    }
}

table 50000 MyTable
{
    Caption = '', Locked = true;
    InherentPermissions = r;

    fields
    {
        field(1; MyField; Integer)
        {
            Caption = '', Locked = true;
            DataClassification = ToBeClassified;
        }
        field(2; MyField2; Integer)
        {
            Caption = '', Locked = true;
            DataClassification = ToBeClassified;
        }
    }
}