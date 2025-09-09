table 50101 "My Table"
{
    Caption = 'MyTable';
    DataClassification = CustomerContent;

    fields
    {
        field(1; [|TestField|]; Code[10])
        {
            Caption = 'TestField';
        }
        field(2; [|AnotherField2|]; Code[10])
        {
            Caption = 'AnotherField2';
        }
    }
}