using Microsoft.VisualStudio.TestTools.UnitTesting;

using NetCoreAudio;

namespace NetCoreAudioTests
{
    [TestClass]
    public class EndpointTests
    {
        [TestMethod]
        public void TestEndpointVolumeAndMute()
        {
            using AudioEndpointManager m = new AudioEndpointManager();
            Assert.IsNotNull(m);
            AudioEndpoint e = m.DefaultEndpoint(EndpointRole.Console, EndpointDataFlow.Render);
            Assert.IsNotNull(e);
            Assert.IsNotNull(e.Id);
            Assert.AreNotEqual("", e.Id);

            Assert.IsFalse(e.VolumeControl.IsMuted);
            Assert.AreNotEqual(-1, e.VolumeControl.Level);
        }
    }
}
