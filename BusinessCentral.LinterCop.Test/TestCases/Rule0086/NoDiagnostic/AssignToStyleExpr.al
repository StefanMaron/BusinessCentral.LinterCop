page 50100 MyPage
{
    layout
    {
        area(Content)
        {
            field(MyField; MyText)
            {
                ApplicationArea = All;
                StyleExpr = [|'Standard'|];
            }
        }
    }

    var
        MyText: Text;
}