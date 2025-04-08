page 50000 MyPage
{
    actions
    {
        area(Processing)
        {
            action(EditInExcel)
            {
                ApplicationArea = All;
                AccessByPermission = system [|"Allow Action Export To Excel"|] = X;
            }
        }
    }
}