# Quick Start

XView can be used in two ways:

1. XView.dll merged with your TOM.NET templating class library dll using ILMerge. (Download ILMerge here: http://www.microsoft.com/en-us/download/details.aspx?id=17630)
2. XView.dll installed on the server's GAC using gacutil.

Follow the instructions in the steps below to get started with XView.

## Step 1. Create new C# Class Library project in Visual Studio
In this step select the appropriate target .NET framework version.

* .NET Framework 4.5.2 -> Web 8
* .NET Framework 4.5 -> Tridion 2013
* .NET Framework 4 -> Tridion 2011

Add references to the following dlls:

* XView.dll
* Tridion.Common.dll
* Tridion.ContentManager.dll
* Tridion.ContentManager.Common.dll
* Tridion.ContentManager.Publishing.dll
* Tridion.ContentManager.Templating.dll

## Step 2. Add Controllers folder and Controller derived class
Add a new folder _**Controllers**_ to your project. In this folder add a new class.
In our example we name the class file _**SampleController.cs**_. Below is the C# code for this class. 

```csharp
using System;
using XView;

namespace Front.Tridion.Templating.Samples.Controllers
{
    public class SampleController : Controller<TridionContext>
    {
        protected override Type GetInternalType(string typeFullName)
        {
            return Type.GetType(typeFullName);
        }
    }
}
```
_If you named your class and project's namespace differently then change the class name and namespace in the code above to reflect your situation._

Add to your project's post-build event the post-build script below which calls TCMUploadAssembly.exe to (merge and) upload your project's dll into Tridion.

Example post-build script for when merging your project's dll with XView.dll.
```code
"$(SolutionDir)Dependencies\ILMerge\ILMerge.exe" /targetplatform:v4,"c:\Windows\Microsoft.NET\Framework\v4.0.30319" /out:"$(TargetDir)$(TargetName).Merged.dll" "$(TargetPath)" "$(TargetDir)XView.dll"
"$(SolutionDir)Dependencies\Tridion\TCMUploadAssembly.exe" /folder:tcm:1-5-2 "$(SolutionDir)Dependencies\Tridion\config.xml" "$(TargetDir)$(TargetName).Merged.dll"
```

Example post-build script for when XView.dll is install on the servers' GAC:
```code
"$(SolutionDir)Dependencies\Tridion\TCMUploadAssembly.exe" /folder:tcm:1-5-2 "$(SolutionDir)Dependencies\Tridion\config.xml" "$(TargetPath)"
```
_Change the path to ILMerge.exe, TCMUploadAssembly.exe, config.xml and the folder tcmuri to match your folders layout._

**Rebuild** your project.

If all went well then your project's dll is uploaded into Tridion after the rebuild action.

## Step 3. Create new Component Template in Template Builder
Open Template Builder and create a new Component Template (CT). In our example we give this CT the name _**Sample**_.

In Template Builder, at the left-hand side you'll see the _**SampleController**_ (and the project's dll) in the Building Blocks panel.

Drag the _**SampleController**_ building block onto the new CT. Hit the Run button, then select a random component to preview.

What you'll see in the resulting output is the output of the XView's ControllerView. The ControllerView tells you either of the two:

1. Change the template name because its name is invalid. In this case create a new CT with a valid name.
1. You need to implement the missing view. In this case proceed with the instruction.

## Step 4. Add new View class
Implement the missing _**SampleView**_. Add a new _**SampleView.cs**_ to your project in the location specified by the ControllerView. Below is the C# code for the _**SampleView**_ class.

```csharp
using Tridion.ContentManager.ContentManagement;
using XView;

namespace Front.Tridion.Templating.Samples.Views.ComponentViews
{
    public class SampleView : View<TridionContext, Component>
    {
        protected override string Render()
        {
            return string.Format("Rendered component: {0}", Model.Title);
        }
    }
}
```
_If you named your class and project's namespace differently then change the class name and namespace in the code above to reflect your situation._

**Rebuild** your project again and re-run in Template Builder.

If things went well you'll see an output produced by the SampleView.

That's it. Have fun coding.
