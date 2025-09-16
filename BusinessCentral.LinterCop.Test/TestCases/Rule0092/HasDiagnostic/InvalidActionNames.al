page 50101 MyPageInvalid
{
    layout { }

    actions
    {
        area(Processing)
        {
            action([|post|]) { }       // violates allow (must start uppercase)
            action([|A42Run|]) { }     // violates disallow (^A42)
            action([|Run_Action|]) { } // underscore not allowed
        }
    }
}