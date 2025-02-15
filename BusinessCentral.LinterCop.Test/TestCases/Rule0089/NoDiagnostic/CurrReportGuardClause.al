report 50100 MyReport
{
    trigger [|OnInitReport|]()  // Cognitive Complexity: 3 (threshold >=15)
    begin
        if true then            // +0 Guard Clause
            CurrReport.Break();
        if true then            // +0 Guard Clause
            CURRREPORT.Break();
        if true then            // +0 Guard Clause
            CurrReport.BREAK();
        if true then            // +0 Guard Clause
            CurrReport.Skip();
        if true then            // +0 Guard Clause
            CURRREPORT.Skip();
        if true then            // +0 Guard Clause
            CurrReport.SKIP();
        if true then            // +0 Guard Clause
            CurrReport.Quit();
        if true then            // +0 Guard Clause
            CURRREPORT.Quit();
        if true then            // +0 Guard Clause
            CurrReport.QUIT();
        if true then            // +1 (nesting = 0) IfStatement
            if true then        // +2 (nesting = 1) IfStatement
                if true then    // +0 Guard Clause   
                    CurrReport.Skip();
    end;
}