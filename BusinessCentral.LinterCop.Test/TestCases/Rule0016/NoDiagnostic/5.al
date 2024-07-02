table 50100 MyTable
{
    DataClassification = ToBeClassified;

    fields
    {
        field(1; MyField; Code[10])
        { 
            Caption = 'My Field';
            DataClassification = ToBeClassified;
        }
    }

    keys
    {
        key(Key1; MyField)
        {
            Clustered = true;
        }
    }
}

page 50100 MyPage
{
    SourceTable = MyTable;
    
    layout
    {
        area(Content)
        {
            [|field(MyField;Rec.MyField)|]
            {
            }
        }
    }
}