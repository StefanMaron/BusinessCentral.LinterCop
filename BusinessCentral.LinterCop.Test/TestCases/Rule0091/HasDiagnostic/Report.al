report 1 ReportCaption
{
    [|Caption|] = 'ReportCaption_Caption';

    requestpage
    {
        [|Caption|] = 'RequestPageCaption_RequestPage';
        layout
        {
            area(Content)
            {
                field(RequestPageField; '')
                {
                    [|Caption|] = 'RequestPageFieldCaption_RequestPageField';
                    [|ToolTip|] = 'RequestPageFieldToolTip_RequestPageField';
                }
            }
        }
        actions
        {
            area(Creation)
            {
                action(Insert)
                {
                    [|Caption|] = 'InsertCaption_Insert';
                    [|ToolTip|] = 'InsertToolTip_Insert';
                }
            }
        }
    }

    labels
    {
        [|Report_labels|] = 'Report_labels';
    }
}
