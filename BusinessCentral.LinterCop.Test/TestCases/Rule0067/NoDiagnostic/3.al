﻿table 50100 MyTable
{
    fields
    {
        [|field(1; MyField; Code[20])|]
        {
            NotBlank = false;
        }
    }

    keys
    {
        key(Key1; MyField)
        {
            Clustered = true;
        }
    }
}