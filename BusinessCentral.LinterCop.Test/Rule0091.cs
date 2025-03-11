#if !LessThenFall2024
using Microsoft.Dynamics.Nav.CodeAnalysis;

namespace BusinessCentral.LinterCop.Test;

public class Rule0091
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0091");
    }

    #region HasDiagnostic

    [Test]
    [TestCase("LocalLabel")]
    public async Task LocalLabel(string testCase)
    {
         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Codeunit 947829021 - Method 3998599243 - NamedType 1457054730"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>LocalLabel_MyProcedure_Label</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Codeunit LocalLabel - Method MyProcedure - NamedType Label</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }


    [Test]
    [TestCase("GlobalLabel")]
    public async Task GlobalLabel(string testCase)
    {
         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Codeunit 1626783127 - NamedType 1457054730"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>GlobalLabel_Label</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Codeunit GlobalLabel - NamedType Label</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }

    [Test]
    [TestCase("Table")]
    public async Task Table(string testCase)
    {
        // TableCaption, TableFieldCaption, TableFieldToolTip

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Table 3139551198 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>TableCaption_Caption</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Table TableFieldCaption - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Table 3139551198 - Field 1296262074 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>TableFieldToolTip_MyField_ToolTip</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Table TableFieldCaption - Field MyField - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Table 3139551198 - Field 1296262074 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>TableFieldCaption_MyField_Caption</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Table TableFieldCaption - Field MyField - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }


    [Test]
    [TestCase("Page")]
    public async Task Page(string testCase)
    {
        // PageCaption, PageGroup, PageFieldCaption, PageFieldToolTip, PageActionCaption, PageActionToolTip

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Page 501693335 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>PageCaption_Caption</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Page PageCaption - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Page 501693335 - Control 739346273 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>PageGroup_Group</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Page PageCaption - Control Group - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Page 501693335 - Control 2257230523 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>PageFieldTooltip_PageField</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Page PageCaption - Control PageField - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Page 501693335 - Control 2257230523 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>PageCaption_PageField</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Page PageCaption - Control PageField - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Page 501693335 - Action 2279323338 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>PageActionToolTip_Action_Caption</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Page PageCaption - Action Action - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Page 501693335 - Action 2279323338 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>PageActionCaption_Action_Caption</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Page PageCaption - Action Action - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }


    [Test]
    [TestCase("Query")]
    public async Task Query(string testCase)
    {
        // QueryCaption, TODO: QueryColumnCaption

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Query 1081262808 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>QueryCaption_Caption</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Query QueryCaption - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Query 1081262808 - QueryColumn 287807385 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>Field1_Caption</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Query QueryCaption - QueryColumn MyField1 - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }

    [Test]
    [TestCase("Profile")]
    public async Task Profile(string testCase)
    {
        // ProfileCaption

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Profile 2849468335 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>ProfileCaption_Caption</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Profile ProfileCaption - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }

    [Test]
    [TestCase("PermissionSet")]
    public async Task PermissionSet(string testCase)
    {
        // PermissionSetCaption

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""PermissionSet 286749945 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>PermissionSetCaption_Caption</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">PermissionSet PermissionSetCaption - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }

    [Test]
    [TestCase("TableExtension")]
    public async Task TableExtension(string testCase)
    {
        // Caption, FieldCaption, FieldToolTip

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Table 1500409770 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Table 1500409770"">
                    <source>TableExt_Caption</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">TableExtension TableExt - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Table 1500409770 - Field 3945078064 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Table 1500409770"">
                    <source>TableFieldToolTip_MyField2_ToolTip</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">TableExtension TableExt - Field MyField2 - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Table 1500409770 - Field 3945078064 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Table 1500409770"">
                    <source>TableFieldCaption_MyField2_Caption</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">TableExtension TableExt - Field MyField2 - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }


    [Test]
    [TestCase("PageExtension")]
    public async Task PageExtension(string testCase)
    {
        // Caption, GroupCaption, FieldCaption, FieldToolTip, ActionCaption, ActionToolTip

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Page 438909893 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Page 438909893"">
                    <source>PageExt_Caption</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">PageExtension PageExt - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Page 438909893 - Action 1135900470 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Page 438909893"">
                    <source>PageActionToolTip_MyAction_ToolTip</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">PageExtension PageExt - Action MyAction - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Page 438909893 - Action 1135900470 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Page 438909893"">
                    <source>PageActionCaption_MyAction_Caption</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">PageExtension PageExt - Action MyAction - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Page 438909893 - Control 894378861 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Page 438909893"">
                    <source>PageExt_GroupCaption_MyGroup_Caption</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">PageExtension PageExt - Control MyGroup - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Page 438909893 - Control 1296262074 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Page 438909893"">
                    <source>PageExt_FieldToolTip_MyField_ToolTip</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">PageExtension PageExt - Control MyField - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Page 438909893 - Control 1296262074 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Page 438909893"">
                    <source>PageExt_FieldCaption_MyField_Caption</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">PageExtension PageExt - Control MyField - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }
    

    [Test]
    [TestCase("Report")]
    public async Task Report(string testCase)
    {
        // Caption, ReportLabel, RequestPageCaption, RequestPageFieldCaption, RequestPageFieldToolTip, RequestPageActionCaption, RequestPageActionToolTip

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Report 253704284 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>ReportCaption_Caption</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Report ReportCaption - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Report 253704284 - ReportLabel 2191183944"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>Report_labels</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Report ReportCaption - ReportLabel Report_labels</note>
                    </trans-unit>
                    <trans-unit id=""Report 253704284 - RequestPage 2516438534 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>RequestPageCaption_RequestPage</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Report ReportCaption - RequestPage RequestOptionsPage - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Report 253704284 - Control 2120039692 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>RequestPageFieldToolTip_RequestPageField</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Report ReportCaption - Control RequestPageField - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Report 253704284 - Control 2120039692 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>RequestPageFieldCaption_RequestPageField</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Report ReportCaption - Control RequestPageField - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Report 253704284 - Action 2049086889 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>InsertToolTip_Insert</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Report ReportCaption - Action Insert - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Report 253704284 - Action 2049086889 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>InsertCaption_Insert</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Report ReportCaption - Action Insert - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }

    [Test]
    [TestCase("ReportExtension")]
    public async Task ReportExtension(string testCase)
    {
        // ReportLabel, RequestPageFieldCaption, RequestPageFieldToolTip,
        //  TODO: RequestPageActionCaption, RequestPageActionToolTip
        // for some strange reason, report extension request page actions also do not get translated by the AL Language?

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Report 253704284 - ReportLabel 2191183944"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Report 253704284"">
                    <source>Report_labels</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">ReportExtension ReportCaptionExt - ReportLabel Report_labels</note>
                    </trans-unit>
                    <trans-unit id=""Report 253704284 - Control 2120039692 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Report 253704284"">
                    <source>RequestPageFieldToolTip_RequestPageField</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">ReportExtension ReportCaptionExt - Control RequestPageField - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Report 253704284 - Control 2120039692 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Report 253704284"">
                    <source>RequestPageFieldCaption_RequestPageField</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">ReportExtension ReportCaptionExt - Control RequestPageField - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }
    

    [Test]
    [TestCase("Enum")]
    public async Task Enum(string testCase)
    {
        // Caption, EnumValueCaption

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Enum 3692015037 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>Enum</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Enum Enum - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Enum 3692015037 - EnumValue 3592160600 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>One</source>
                    <target state=""needs-translation""/>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Enum Enum - EnumValue One - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }
    #endregion

    #region Has Translations

    [Test]
    [TestCase("LocalLabel")]
    public async Task LocalLabel_Translated(string testCase)
    {
         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Codeunit 947829021 - Method 3998599243 - NamedType 1457054730"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>LocalLabel_MyProcedure_Label</source>
                    <target>LocalLabel_MyProcedure_Label</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Codeunit LocalLabel - Method MyProcedure - NamedType Label</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }


    [Test]
    [TestCase("GlobalLabel")]
    public async Task GlobalLabel_Translated(string testCase)
    {
         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Codeunit 1626783127 - NamedType 1457054730"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>GlobalLabel_Label</source>
                    <target>GlobalLabel_Label</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Codeunit GlobalLabel - NamedType Label</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }

    [Test]
    [TestCase("Table")]
    public async Task Table_Translated(string testCase)
    {
        // TableCaption, TableFieldCaption, TableFieldToolTip

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Table 3139551198 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>TableCaption_Caption</source>
                    <target>TableCaption_Caption</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Table TableFieldCaption - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Table 3139551198 - Field 1296262074 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>TableFieldToolTip_MyField_ToolTip</source>
                    <target>TableFieldToolTip_MyField_ToolTip</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Table TableFieldCaption - Field MyField - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Table 3139551198 - Field 1296262074 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>TableFieldCaption_MyField_Caption</source>
                    <target>TableFieldCaption_MyField_Caption</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Table TableFieldCaption - Field MyField - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);


        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }


    [Test]
    [TestCase("Page")]
    public async Task Page_Translated(string testCase)
    {
        // PageCaption, PageGroup, PageFieldCaption, PageFieldToolTip, PageActionCaption, PageActionToolTip

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Page 501693335 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>PageCaption_Caption</source>
                    <target>PageCaption_Caption</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Page PageCaption - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Page 501693335 - Control 2257230523 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>PageFieldTooltip_PageField</source>
                    <target>PageFieldTooltip_PageField</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Page PageCaption - Control PageField - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Page 501693335 - Control 2257230523 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>PageCaption_PageField</source>
                    <target>PageCaption_PageField</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Page PageCaption - Control PageField - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Page 501693335 - Control 266656143 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>PageLabelCaption_PageLabel</source>
                    <target>PageLabelCaption_PageLabel</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Page PageCaption - Control PageLabel - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Page 501693335 - Action 2279323338 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>PageActionToolTip_Action_Caption</source>
                    <target>PageActionToolTip_Action_Caption</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Page PageCaption - Action Action - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Page 501693335 - Action 2279323338 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>PageActionCaption_Action_Caption</source>
                    <target>PageActionCaption_Action_Caption</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Page PageCaption - Action Action - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }


    [Test]
    [TestCase("Query")]
    public async Task Query_Translated(string testCase)
    {
        // QueryCaption, TODO: QueryColumnCaption

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Query 1081262808 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>QueryCaption_Caption</source>
                    <target>QueryCaption_Caption</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Query QueryCaption - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Query 1081262808 - QueryColumn 287807385 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>Field1_Caption</source>
                    <target>Field1_Caption</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Query QueryCaption - QueryColumn MyField1 - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }

    [Test]
    [TestCase("Profile")]
    public async Task Profile_Translated(string testCase)
    {
        // ProfileCaption

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Profile 2849468335 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>ProfileCaption_Caption</source>
                    <target>ProfileCaption_Caption</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Profile ProfileCaption - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }

    [Test]
    [TestCase("PermissionSet")]
    public async Task PermissionSet_Translated(string testCase)
    {
        // PermissionSetCaption

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""PermissionSet 286749945 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>PermissionSetCaption_Caption</source>
                    <target>PermissionSetCaption_Caption</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">PermissionSet PermissionSetCaption - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }

    [Test]
    [TestCase("TableExtension")]
    public async Task TableExtension_Translated(string testCase)
    {
        // Caption, FieldCaption, FieldToolTip

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Table 1500409770 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Table 1500409770"">
                    <source>TableExt_Caption</source>
                    <target>TableExt_Caption</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">TableExtension TableExt - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Table 1500409770 - Field 3945078064 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Table 1500409770"">
                    <source>TableFieldToolTip_MyField2_ToolTip</source>
                    <target>TableFieldToolTip_MyField2_ToolTip</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">TableExtension TableExt - Field MyField2 - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Table 1500409770 - Field 3945078064 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Table 1500409770"">
                    <source>TableFieldCaption_MyField2_Caption</source>
                    <target>TableFieldCaption_MyField2_Caption</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">TableExtension TableExt - Field MyField2 - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }


    [Test]
    [TestCase("PageExtension")]
    public async Task PageExtension_Translated(string testCase)
    {
        // Caption, GroupCaption, FieldCaption, FieldToolTip, ActionCaption, ActionToolTip

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Page 438909893 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Page 438909893"">
                    <source>PageExt_Caption</source>
                    <target>PageExt_Caption</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">PageExtension PageExt - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Page 438909893 - Action 1135900470 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Page 438909893"">
                    <source>PageActionToolTip_MyAction_ToolTip</source>
                    <target>PageActionToolTip_MyAction_ToolTip</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">PageExtension PageExt - Action MyAction - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Page 438909893 - Action 1135900470 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Page 438909893"">
                    <source>PageActionCaption_MyAction_Caption</source>
                    <target>PageActionCaption_MyAction_Caption</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">PageExtension PageExt - Action MyAction - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Page 438909893 - Control 894378861 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Page 438909893"">
                    <source>PageExt_GroupCaption_MyGroup_Caption</source>
                    <target>PageExt_GroupCaption_MyGroup_Caption</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">PageExtension PageExt - Control MyGroup - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Page 438909893 - Control 1296262074 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Page 438909893"">
                    <source>PageExt_FieldToolTip_MyField_ToolTip</source>
                    <target>PageExt_FieldToolTip_MyField_ToolTip</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">PageExtension PageExt - Control MyField - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Page 438909893 - Control 1296262074 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Page 438909893"">
                    <source>PageExt_FieldCaption_MyField_Caption</source>
                    <target>PageExt_FieldCaption_MyField_Caption</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">PageExtension PageExt - Control MyField - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }
    

    [Test]
    [TestCase("Report")]
    public async Task Report_Translated(string testCase)
    {
        // Caption, ReportLabel, RequestPageCaption, RequestPageFieldCaption, RequestPageFieldToolTip, RequestPageActionCaption, RequestPageActionToolTip

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Report 253704284 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>ReportCaption_Caption</source>
                    <target>ReportCaption_Caption</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Report ReportCaption - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Report 253704284 - ReportLabel 2191183944"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>Report_labels</source>
                    <target>Report_labels</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Report ReportCaption - ReportLabel Report_labels</note>
                    </trans-unit>
                    <trans-unit id=""Report 253704284 - RequestPage 2516438534 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>RequestPageCaption_RequestPage</source>
                    <target>RequestPageCaption_RequestPage</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Report ReportCaption - RequestPage RequestOptionsPage - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Report 253704284 - Control 2120039692 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>RequestPageFieldToolTip_RequestPageField</source>
                    <target>RequestPageFieldToolTip_RequestPageField</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Report ReportCaption - Control RequestPageField - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Report 253704284 - Control 2120039692 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>RequestPageFieldCaption_RequestPageField</source>
                    <target>RequestPageFieldCaption_RequestPageField</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Report ReportCaption - Control RequestPageField - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Report 253704284 - Action 2049086889 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>InsertToolTip_Insert</source>
                    <target>InsertToolTip_Insert</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Report ReportCaption - Action Insert - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Report 253704284 - Action 2049086889 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>InsertCaption_Insert</source>
                    <target>InsertCaption_Insert</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Report ReportCaption - Action Insert - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }

    [Test]
    [TestCase("ReportExtension")]
    public async Task ReportExtension_Translated(string testCase)
    {
        // ReportLabel, RequestPageFieldCaption, RequestPageFieldToolTip,
        //  TODO: RequestPageActionCaption, RequestPageActionToolTip
        // for some strange reason, report extension request page actions also do not get translated by the AL Language?

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Report 253704284 - ReportLabel 2191183944"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Report 253704284"">
                    <source>Report_labels</source>
                    <target>Report_labels</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">ReportExtension ReportCaptionExt - ReportLabel Report_labels</note>
                    </trans-unit>
                    <trans-unit id=""Report 253704284 - Control 2120039692 - Property 1295455071"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Report 253704284"">
                    <source>RequestPageFieldToolTip_RequestPageField</source>
                    <target>RequestPageFieldToolTip_RequestPageField</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">ReportExtension ReportCaptionExt - Control RequestPageField - Property ToolTip</note>
                    </trans-unit>
                    <trans-unit id=""Report 253704284 - Control 2120039692 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"" al-object-target=""Report 253704284"">
                    <source>RequestPageFieldCaption_RequestPageField</source>
                    <target>RequestPageFieldCaption_RequestPageField</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">ReportExtension ReportCaptionExt - Control RequestPageField - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }
    

    [Test]
    [TestCase("Enum")]
    public async Task Enum_Translated(string testCase)
    {
        // Caption, EnumValueCaption

         string xliffContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                <group id=""body"">
                    <trans-unit id=""Enum 3692015037 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>Enum</source>
                    <target>Enum</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Enum Enum - Property Caption</note>
                    </trans-unit>
                    <trans-unit id=""Enum 3692015037 - EnumValue 3592160600 - Property 2879900210"" size-unit=""char"" translate=""yes"" xml:space=""preserve"">
                    <source>One</source>
                    <target>One</target>
                    <note from=""Developer"" annotates=""general"" priority=""2""/>
                    <note from=""Xliff Generator"" annotates=""general"" priority=""3"">Enum Enum - EnumValue One - Property Caption</note>
                    </trans-unit>
                </group>
                </body>
            </file>
            </xliff>
        ";

        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }
    #endregion

    #region Edge Cases

    private string emptyXliff = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
            <file datatype=""xml"" source-language=""en-US"" target-language=""de-DE"" original=""ALProject1"">
                <body>
                </body>
            </file>
            </xliff>
    ";

    [Test]
    [TestCase("LocalLabel")]
    [TestCase("GlobalLabel")]
    [TestCase("Table")]
    [TestCase("Page")]
    [TestCase("Query")]
    [TestCase("Profile")]
    [TestCase("PermissionSet")]
    [TestCase("TableExtension")]
    [TestCase("PageExtension")]
    [TestCase("ReportExtension")]
    [TestCase("Report")]
    [TestCase("Enum")]
    public async Task EmptyTranslation(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        IEnumerable<Stream>? xliffFileStream = new List<Stream> { new MemoryStream(System.Text.Encoding.UTF8.GetBytes(emptyXliff)) };

        Rule0091LabelsShouldBeTranslated rule = new Rule0091LabelsShouldBeTranslated();
        rule.UpdateCache(xliffFileStream);
        rule.DoNotUpdateCache = true;

        var fixture = RoslynFixtureFactory.Create(rule);
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated.Id);
    }

    #endregion
}
#endif