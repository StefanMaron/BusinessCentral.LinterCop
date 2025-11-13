table 70000 "MyTable"
{
    DataClassification = ToBeClassified;

    fields
    {
        field(1; "Id"; Integer)
        {
            DataClassification = ToBeClassified;
        }
    }

    keys
    {
        key(PK; "Id")
        {
            Clustered = true;
        }
    }

    procedure OpenPageModal()
    begin
        Page.RunModal(Page::"MyPage", [|Rec|]);
    end;
}

page 70002 "MyPage"
{
    PageType = Card;
    SourceTable = "MyTable";
    ApplicationArea = All;

    layout
    {
        area(content)
        {
            group(General)
            {
                field(Id; Rec.Id) { ApplicationArea = All; }
            }
        }
    }
}