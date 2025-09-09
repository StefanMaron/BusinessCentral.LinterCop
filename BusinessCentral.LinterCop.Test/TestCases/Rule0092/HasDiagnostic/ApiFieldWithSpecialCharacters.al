table 50100 "My Table"
{
    Caption = 'MyTable';
    DataClassification = CustomerContent;

    fields
    {
        field(1; "No."; Code[20]) { }
        field(2; Name; Text[100]) { }
    }

    keys
    {
        key(PK; "No.") { Clustered = true; }
    }
}

page 60101 "My Api Page Invalid"
{
    PageType = API;
    SourceTable = "My Table";
    APIPublisher = 'contoso';
    APIGroup = 'test';
    APIVersion = 'v1.0';
    EntityName = 'myEntity2';
    EntitySetName = 'myEntities2';
    ODataKeyFields = SystemId;
    DelayedInsert = true;

    layout
    {
        area(content)
        {
            repeater(Group)
            {
                field([|MyField|]; Name) { }          // starts uppercase (invalid)
                field([|my_field|]; Name) { }         // underscore (invalid)
                field([|MyField2|]; "No.") { }        // starts uppercase (invalid)
                field([|my_field2|]; "No.") { }       // underscore (invalid)
            }
        }
    }
}