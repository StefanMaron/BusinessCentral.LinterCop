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

page 60100 "My Api Page Valid"
{
    PageType = API;
    SourceTable = "My Table";
    APIPublisher = 'contoso';
    APIGroup = 'test';
    APIVersion = 'v1.0';
    EntityName = 'myEntity';
    EntitySetName = 'myEntities';
    ODataKeyFields = SystemId;
    DelayedInsert = true;

    layout
    {
        area(content)
        {
            repeater(Group)
            {
                field([|myfield|]; Name) { }
                field([|myField1|]; "No.") { }
            }
        }
    }
}