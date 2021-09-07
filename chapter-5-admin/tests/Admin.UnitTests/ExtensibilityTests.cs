using Admin.Extensibility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace Admin.UnitTests
{
    public class ExtensibilityTests
    {


        [Fact]
        public void Hook_CallsCorrectMethod_NoParameters()
        {
            // Arrange
            string execLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string extensionLocation = Path.Combine(execLocation, @"..\..\..\..\..\extended\CommitCustomerDataExtended");            

            File.Copy(Path.Combine(extensionLocation, @"CommitCustomerDataExtended\bin\debug\net5.0\CommitCustomerDataExtended.dll"), 
                Path.Combine(execLocation, "CommitCustomerDataExtended.dll"), true);
            var hook = new Hook();

            // Act
            hook.CreateHook("After", "CommitCustomerData");

            // Assert

            
        }
    }
}
