table 50102 "My Table"
{
    Caption = 'MyTable';
    DataClassification = CustomerContent;

    fields
    {
        field(1; [|testfield|]; Code[10]) // starts with lowercase, violates allow pattern
        {
            Caption = 'testfield';
        }
        field(2; [|A42Field|]; Code[10]) // starts with A42, violates disallow pattern
        {
            Caption = 'A42Field';
        }
        field(3; [|Field_With_Special|]; Code[10]) // contains underscore, violates allow pattern
        {
            Caption = 'Field_With_Special';
        }
    }
}