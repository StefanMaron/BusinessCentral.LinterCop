codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        JobQueueCategory: Record "Job Queue Category";
        JobQueueCategoryCodeLbl: Label 'MyCategory', Locked = true;
    begin
        [|JobQueueCategory.Get(JobQueueCategoryCodeLbl)|];
    end;
}

table 50100 "Job Queue Category"
{
    fields
    {
        field(1; "Code"; Code[10]) { }
    }
}