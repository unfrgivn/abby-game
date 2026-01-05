using NUnit.Framework;
using WildsOfCloverhollow.Bootstrap;

namespace WildsOfCloverhollow.Tests.EditMode
{
    [TestFixture]
    public class InputRouterTests
    {
        [Test]
        public void InputMode_Gameplay_HasExpectedValue()
        {
            Assert.That((int)InputRouter.InputMode.Gameplay, Is.EqualTo(0));
        }

        [Test]
        public void InputMode_UI_HasExpectedValue()
        {
            Assert.That((int)InputRouter.InputMode.UI, Is.EqualTo(1));
        }

        [Test]
        public void InputMode_EnumValues_AreDistinct()
        {
            Assert.That(InputRouter.InputMode.Gameplay, Is.Not.EqualTo(InputRouter.InputMode.UI));
        }
    }
}
