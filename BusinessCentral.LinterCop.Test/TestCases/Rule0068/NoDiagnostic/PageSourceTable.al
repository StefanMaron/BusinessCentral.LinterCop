page 50000 MyPage
{
    PageType = Card;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = MyTable;

    layout
    {
        area(Content)
        {
            group(GroupName)
            {
                field(Name; MyField)
                {
                    ApplicationArea = All;

                }
            }
        }
    }

    trigger OnOpenPage()
    var
        MyTable: Record MyTable;
    begin
        [|Rec.FindFirst();|]
        [|MyTable.FindFirst();|]
        [|MyTable.Insert();|]
        [|MyTable.Modify();|]
        [|MyTable.Delete();|]
    end;
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