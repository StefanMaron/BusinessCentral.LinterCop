page 50100 MyPage
{
    Caption = [|'Unfavorable'|], Locked = true;

    layout
    {
        area(Content)
        {
            group(Unfavorable)
            {
                Caption = [|'Unfavorable'|], Locked = true;

                field(MyField; MyField)
                {
                    ApplicationArea = All;
                    Caption = [|'Unfavorable'|], Locked = true;
                }
            }

        }
    }

    var
        MyField, MyFieldStyle : Text;

    trigger OnAfterGetRecord()
    begin
        MyFieldStyle := Format(PageStyle::[|Unfavorable|]);
    end;
}