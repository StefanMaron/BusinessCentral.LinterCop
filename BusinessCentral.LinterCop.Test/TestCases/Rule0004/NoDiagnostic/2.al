[|table 50100 MyTable|]
{
    LookupPageId = MyPage;
    DrillDownPageId = MyPage;
    DataClassification = ToBeClassified;

    fields
    {
        field(1; MyField; Integer)
        {
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
    PageType = List;
    SourceTable = MyTable;
    SourceTableTemporary = true;

    layout
    {
    }
}