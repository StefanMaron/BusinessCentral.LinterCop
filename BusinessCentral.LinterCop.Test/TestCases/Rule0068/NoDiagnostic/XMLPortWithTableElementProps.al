xmlport 50000 MyXmlport
{
    Direction = Import;

    schema
    {
        textelement(NodeName1)
        {
            [|tableelement(NodeName2; MyTable)|]
            {
                AutoReplace = false; // modify permissions
                AutoSave = false; // insert permissions
                AutoUpdate = false; //modify permissions 

                fieldattribute(NodeName3; NodeName2.MyField)
                {

                }
            }
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
        field(2; MyField2; Integer)
        {
            Caption = '', Locked = true;
            DataClassification = ToBeClassified;
        }
    }
}