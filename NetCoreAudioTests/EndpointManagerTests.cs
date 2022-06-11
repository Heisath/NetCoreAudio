using Microsoft.VisualStudio.TestTools.UnitTesting;

using NetCoreAudio;
using System.Collections.Generic;

namespace NetCoreAudioTests
{
    [TestClass]
    public class EndpointMangerTests
    {
        [TestMethod]
        public void TestEndpointManagerDefault()
        {
            using AudioEndpointManager m = new AudioEndpointManager();
            Assert.IsNotNull(m);
            AudioEndpoint e = m.DefaultEndpoint(EndpointRole.Console, EndpointDataFlow.Render);
            Assert.IsNotNull(e.Id);
            Assert.AreNotEqual("", e.Id);
        }
        [TestMethod]
        public void TestEndpointManagerName()
        {
            using AudioEndpointManager m = new AudioEndpointManager();
            Assert.IsNotNull(m);
            List<AudioEndpoint> e = m.EndpointByName("Lautsprecher", EndpointDataFlow.Render, EndpointState.Active);
            Assert.IsNotNull(e);
            Assert.AreNotEqual(0, e.Count);
        }
    }
}
