page 50100 MyPage
{
    layout
    {
        area(Content)
        {
            field(MyField; MyField)
            {
                ApplicationArea = All;
                StyleExpr = MyFieldStyle;
            }
        }
    }

    var
        MyField, MyFieldStyle : Text;

    trigger OnAfterGetRecord()
    begin
        MyFieldStyle := [|'Unfavorable'|];
    end;
}