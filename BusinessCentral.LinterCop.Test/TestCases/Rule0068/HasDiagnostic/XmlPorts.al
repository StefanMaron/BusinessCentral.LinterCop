xmlport 50000 MyXmlport
{
    Direction = Import;
    Permissions = tabledata MyTable = r;

    schema
    {
        textelement(NodeName1)
        {
            [|tableelement(NodeName2; MyTable)|]
            {
                fieldattribute(NodeName3; NodeName2.MyField2)
                {

                }
            }
        }
    }
}
xmlport 50001 MyXmlport2
{
    Direction = Export;
    Permissions = tabledata MyTable = m;

    schema
    {
        textelement(NodeName1)
        {
            [|tableelement(NodeName2; MyTable)|]
            {
                fieldattribute(NodeName3; NodeName2.MyField2)
                {

                }
            }
        }
    }
}
xmlport 50002 MyXmlport3
{
    Direction = Both;

    schema
    {
        textelement(NodeName1)
        {
            [|tableelement(NodeName2; MyTable)|]
            {
                fieldattribute(NodeName3; NodeName2.MyField2)
                {

                }
            }
        }
    }
}

xmlport 50003 MyXmlport4
{

    schema
    {
        textelement(NodeName1)
        {
            [|tableelement(NodeName2; MyTable)|]
            {
                fieldattribute(NodeName3; NodeName2.MyField2)
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