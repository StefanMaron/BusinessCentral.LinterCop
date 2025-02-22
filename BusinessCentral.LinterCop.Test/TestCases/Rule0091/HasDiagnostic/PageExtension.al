pageextension 1 PageExt extends Page
{
    [|Caption|] = 'PageExt_Caption';

    layout
    {
        addfirst(Content)
        {
            group(MyGroup)
            {
                [|Caption|] = 'PageExt_GroupCaption_MyGroup_Caption';

                field(MyField; '')
                {
                    [|Caption|] = 'PageExt_FieldCaption_MyField_Caption';
                    [|ToolTip|] = 'PageExt_FieldToolTip_MyField_ToolTip';
                }
            }
        }
    }

    actions
    {
        addfirst(Processing)
        {
            action(MyAction)
            {
                [|Caption|] = 'PageActionCaption_MyAction_Caption';
                [|ToolTip|] = 'PageActionToolTip_MyAction_ToolTip';
            }
        }
    }
}

page 1 Page { }