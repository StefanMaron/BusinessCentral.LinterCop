page 50100 MyPage
{
    ApplicationArea = All;

    layout
    {
        area(content)
        {
            field(MyField; MyField)
            {
                [|ApplicationArea = Basic;|]
            }
        }
    }

    var
        MyField: Text;
}