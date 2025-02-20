[|table|] 50000 "My Table"
{
    [|Access|] = [|Public|];
    [|Caption|] = 'My Table';
    [|ColumnStoreIndex|] = [|"My Field"|];
    [|CompressionType|] = [|Unspecified|];
    [|DataCaptionFields|] = [|"My Field"|];
    [|DataClassification|] = [|EndUserIdentifiableInformation|];
    [|DataPerCompany|] = [|false|];
    [|Description|] = 'My Description';
    [|Extensible|] = [|true|];
    [|DrillDownPageId|] = [|"My Page"|];
    [|InherentEntitlements|] = RIMDX;
    [|InherentPermissions|] = RIMDX;
    [|LookupPageId|] = [|"My Page"|];
    [|ObsoleteReason|] = 'My ObsoleteReason';
    [|ObsoleteState|] = [|Pending|];
    [|ObsoleteTag|] = 'My ObsoleteTag';
    [|ReplicateData|] = [|false|];
    [|Scope|] = [|Cloud|];
    [|TableType|] = [|Normal|];

    [|fields|]
    {
        [|field|](1; "My Field"; [|Integer|])
        {
            [|Access|] = [|Protected|];
            [|AccessByPermission|] = [|table|] [|"My Table"|] = X;
            [|AllowInCustomizations|] = [|Always|];
            [|AutoFormatExpression|] = '<Currency Code>';
            [|AutoFormatType|] = 2;
            [|AutoIncrement|] = [|false|];
            [|BlankNumbers|] = [|BlankZero|];
            [|BlankZero|] = [|false|];
            [|Caption|] = 'My Caption';
            [|DataClassification|] = [|SystemMetadata|];
            [|Description|] = 'My Description';
            [|Editable|] = [|false|];
            [|Enabled|] = [|true|];
            [|FieldClass|] = [|Normal|];
            [|InitValue|] = 1;
            [|MaxValue|] = 100;
            [|MinValue|] = 1;
            [|NotBlank|] = [|false|];
            [|ToolTip|] = 'My ToolTip';
        }
        [|field|](2; "My FlowField"; [|Integer|])
        {
            [|FieldClass|]= [|FlowField|];
            [|CalcFormula|] = [|max|]([|"My Table"|].[|"My Field"|] [|where|]([|"My Field"|] = [|field|]([|"My Field"|])));
        }
        [|field|](3; "My Blob"; [|Blob|])
        {
            [|Subtype|] = [|Json|];
        }
    }

    [|fieldgroups|]
    {
        [|fieldgroup|](DropDown; [|"My Field"|])
        {
            [|Caption|] = 'My DropDown';
            [|ObsoleteReason|] = 'My ObsoleteReason';
            [|ObsoleteState|] = [|Pending|];
            [|ObsoleteTag|] = 'My ObsoleteTag';
        }
        [|fieldgroup|](Brick; [|"My Field"|], [|"My FlowField"|])
        {
            [|ObsoleteState|] = [|No|];
        }
    }

    [|trigger|] [|OnInsert|]()
    [|begin|]
    [|end|];
    
    [|trigger|] [|OnModify|]()
    [|begin|]
    [|end|];
    
    [|trigger|] [|OnDelete|]()
    [|begin|]
    [|end|];
    
    [|trigger|] [|OnRename|]()
    [|begin|]
    [|end|];
}

page 50000 "My Page" { }