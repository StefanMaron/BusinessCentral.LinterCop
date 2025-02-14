xmlport 50100 MyXmlport
{
    trigger [|OnInitXmlPort|]() // Cognitive Complexity: 3 (threshold >=15)
    begin
        if true then            // +0 Guard Clause
            currXMLport.Break();
        if true then            // +0 Guard Clause
            CURRXMLPORT.Break();
        if true then            // +0 Guard Clause
            currXMLport.BREAK();
        if true then            // +0 Guard Clause
            currXMLport.Skip();
        if true then            // +0 Guard Clause
            CURRXMLPORT.Skip();
        if true then            // +0 Guard Clause
            currXMLport.SKIP();
        if true then            // +0 Guard Clause
            currXMLport.Quit();
        if true then            // +0 Guard Clause
            CURRXMLPORT.Quit();
        if true then            // +0 Guard Clause
            currXMLport.QUIT();
        if true then            // +1 (nesting = 0) IfStatement
            if true then        // +2 (nesting = 1) IfStatement
                if true then    // +0 Guard Clause   
                    currXMLport.Skip();
    end;
}