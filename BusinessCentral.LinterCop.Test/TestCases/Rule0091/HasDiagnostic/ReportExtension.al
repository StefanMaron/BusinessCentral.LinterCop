reportextension 1 ReportCaptionExt extends ReportCaption
{
    requestpage
    {
        layout
        {
            addlast(Content)
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
            addlast(Content)
            {
                action(Insert)
                {
                    Caption = 'InsertCaption_Insert';
                    ToolTip = 'InsertToolTip_Insert';
                }
            }
        }
    }

    labels
    {
        [|Report_labels|] = 'Report_labels';
    }
}

report 1 ReportCaption { }