using Microsoft.VisualStudio.TestTools.UnitTesting;

using NetCoreAudio;
using System.Collections.Generic;

namespace NetCoreAudioTests
{
    [TestClass]
    public class SessionTests
    {
        [TestMethod]
        public void TestSessionGetDefault()
        {
            using AudioEndpointManager m = new AudioEndpointManager();
            Assert.IsNotNull(m);

            AudioEndpoint e = m.DefaultEndpoint(EndpointRole.Console, EndpointDataFlow.Render);
            Assert.IsNotNull(e.Id);
            Assert.AreNotEqual("", e.Id);

            List<AudioSession> l = e.Sessions;
            Assert.IsNotNull(l);
            Assert.AreNotEqual(0, l.Count);

            Assert.IsNotNull(l[0].ProcessTitle);
            
        }
    }
}
