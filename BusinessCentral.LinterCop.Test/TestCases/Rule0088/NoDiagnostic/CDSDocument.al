table 50100 "Dataverse Project PTE"
{
    ExternalName = 'prefix_project';
    TableType = CDS;
    Description = '';
    Caption = 'Project';

    fields
    {
        field(1; prefix_projectId; Guid)
        {
            ExternalName = 'prefix_projectid';
            ExternalType = 'Uniqueidentifier';
            ExternalAccess = Insert;
            Description = 'Unique identifier for entity instances';
            Caption = 'Project';
        }
        field(2; prefix_name; Text[100])
        {
            ExternalName = 'prefix_name';
            ExternalType = 'String';
            Description = 'The name of the custom entity.';
            Caption = 'Name';
        }
        field(25; statecode; [|Option|])
        {
            ExternalName = 'statecode';
            ExternalType = 'State';
            ExternalAccess = Modify;
            Description = 'Status of the Project';
            Caption = 'Status';
            InitValue = " ";
            OptionMembers = " ",Active,Inactive;
            OptionOrdinalValues = -1, 0, 1;
        }
    }
}