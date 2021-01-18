using System.ComponentModel;
using Imgeneus.Core.Extensions;
using Xunit;

namespace Imgeneus.World.Tests.ExtensionTests
{
    public class CharnameValidationTest : BaseTest
    {

        [Theory]
        [Description("Charname shouldn't allow banned tags or")]
        [InlineData("ON", false)]
        [InlineData("ONE", true)]
        [InlineData("TWENTYONECHARACTERNAM", false)]
        [InlineData("TWENTYCHARACTERNAMEE", true)]
        [InlineData("-_-Olive-_-", true)]
        [InlineData("-_-_-_-Olive-_-_-_-", false)]
        [InlineData(".Michael.", true)]
        [InlineData("..Michael..", false)]
        [InlineData(".-Michael-.", true)]
        [InlineData("000Agent000", true)]
        [InlineData("0123Agent0123", false)]
        [InlineData("01234", false)]
        [InlineData("lullabyllel", true)]
        [InlineData("lullabyllell", false)]
        [InlineData("IIIlllIl", false)]
        [InlineData("[GS]Toby", false)]
        [InlineData("GSToby", false)]
        [InlineData("BUGSBUNNY", true)]
        [InlineData("[TGS]Alfred", false)]
        [InlineData("TGSAlfred", false)]
        [InlineData("NUMTGS", true)]
        [InlineData("[GM]Nobody", false)]
        [InlineData("GMNobody", false)]
        [InlineData("SEGMENTATION", true)]
        [InlineData("[GMA]Pure", false)]
        [InlineData("GMAPure", false)]
        [InlineData("STIGMATIZE", true)]
        [InlineData("[ADM]Faith", false)]
        [InlineData("ADMFaith", false)]
        [InlineData("THEADMIRAL", true)]
        [InlineData("[DEV]Fire", false)]
        [InlineData("DEVFire", false)]
        [InlineData("NONDEVIANT", true)]
        [InlineData("[DEVELOPER]Bunny", false)]
        [InlineData("DEVELOPERBunny", false)]
        public void CharacterNameValidationTest(string name, bool isValid)
        {
            Assert.Equal(isValid, name.IsValidCharacterName());
        }
    }
}
