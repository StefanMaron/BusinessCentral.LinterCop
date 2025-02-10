table 50100 "My Table"
{
    fields
    {
        field(1; MyField; Integer)
        {
            DataClassification = ToBeClassified;
        }
        field(2; "Test Option"; [|Option|])
        {
            Caption = 'Test Option';
            ObsoleteState = Pending;
            DataClassification = CustomerContent;
            OptionCaption = '0,1,2,3,4,5';
            OptionMembers = "0","1","2","3","4","5";
        }    
    }
}