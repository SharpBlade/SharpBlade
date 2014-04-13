// ---------------------------------------------------------------------------------------
//  <copyright file="AssemblyInfo.cs" company="SharpBlade">
//      Copyright © 2013-2014 by Adam Hellberg and Brandon Scott.
//
//      Permission is hereby granted, free of charge, to any person obtaining a copy of
//      this software and associated documentation files (the "Software"), to deal in
//      the Software without restriction, including without limitation the rights to
//      use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//      of the Software, and to permit persons to whom the Software is furnished to do
//      so, subject to the following conditions:
//
//      The above copyright notice and this permission notice shall be included in all
//      copies or substantial portions of the Software.
//
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//      WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//      CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//      Disclaimer: SharpBlade is in no way affiliated
//      with Razer and/or any of its employees and/or licensors.
//      Adam Hellberg does not take responsibility for any harm caused, direct
//      or indirect, to any Razer peripherals via the use of SharpBlade.
//
//      "Razer" is a trademark of Razer USA Ltd.
//  </copyright>
// ---------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SharpBlade")]
[assembly: AssemblyDescription("C# wrapper/implementation of the SwitchBlade UI API from Razer")]
#if RELEASE
[assembly: AssemblyConfiguration("Release")]
#else

[assembly: AssemblyConfiguration("Debug")]
#endif

[assembly: AssemblyCompany("SharpBlade")]
[assembly: AssemblyProduct("SharpBlade")]
[assembly: AssemblyCopyright("Copyright © 2013-2014 by Adam Hellberg and Brandon Scott.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("2a168fde-01f2-48df-951f-8d521920de05")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("4.0.0")]
[assembly: AssemblyFileVersion("4.0.0.0")]
[assembly: AssemblyInformationalVersion("4.0.0 Stable")]
