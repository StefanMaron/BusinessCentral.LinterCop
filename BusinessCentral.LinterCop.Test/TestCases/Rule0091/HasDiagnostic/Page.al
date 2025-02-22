page 1 PageCaption
{
    [|Caption|] = 'PageCaption_Caption';
    layout
    {
        area(Content)
        {
            label('PageLabel')
            {
                [|Caption|] = 'PageLabelCaption_PageLabel';
            }
            field(PageField; '')
            {
                [|Caption|] = 'PageCaption_PageField';
                [|ToolTip|] = 'PageFieldTooltip_PageField';
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(Action)
            {
                [|Caption|] = 'PageActionCaption_Action_Caption';
                [|ToolTip|] = 'PageActionToolTip_Action_Caption';
            }
        }
    }
}