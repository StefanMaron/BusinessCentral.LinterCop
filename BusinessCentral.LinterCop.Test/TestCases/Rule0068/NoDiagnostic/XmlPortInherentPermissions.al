xmlport 50000 MyXmlport
{
    Direction = Import;

    schema
    {
        textelement(NodeName1)
        {
            tableelement(NodeName2; MyTable)
            {
                fieldattribute(NodeName3; NodeName2.SourceFieldName)
                {

                }
            }
        }
    }
}
xmlport 50001 MyXmlport2
{
    Direction = Export;

    schema
    {
        textelement(NodeName1)
        {
            tableelement(NodeName2; MyTable)
            {
                fieldattribute(NodeName3; NodeName2.SourceFieldName)
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
            tableelement(NodeName2; MyTable)
            {
                fieldattribute(NodeName3; NodeName2.SourceFieldName)
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
            tableelement(NodeName2; MyTable)
            {
                fieldattribute(NodeName3; NodeName2.SourceFieldName)
                {

                }
            }
        }
    }
}

table 50000 MyTable
{
    Caption = '', Locked = true;
    InherentPermissions = rim;

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