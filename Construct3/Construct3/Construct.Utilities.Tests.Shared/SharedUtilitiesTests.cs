using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Construct.Utilities.Shared;

namespace Construct.Utilities.Tests.Shared
{    
    /// <summary>
    /// Underscores follow by a number represent sequence
    /// "S" followed by a number represents steps
    /// </summary>
    [TestClass]
    public class SharedUtilitiesTests
    {
        public Uri serverEndpoint = null;

        private void CreateStandardConstructServerEndpointUri(string scheme, string hostname, int port = 0)
        {
            serverEndpoint = UriUtility.CreateStandardConstructServiceEndpointUri(scheme, "Server", hostname, Guid.Empty, port);
            Assert.IsNotNull(serverEndpoint);

            string portString = string.Empty;
            if (scheme == "http") portString = string.Format(":{0}", port);

            string expected = string.Format
            (
                "{0}://{1}{2}/{3}/Server/ServerService.svc",
                scheme, hostname, portString, Guid.Empty
            );

            Assert.AreEqual(expected, serverEndpoint.AbsoluteUri);
        }

        /// <summary>
        /// <br />Scheme: net.pipe
        /// <br />Hostname: localhost
        /// </summary>
        [TestMethod]
        public void _1_1_S1_CreateStandardConstructServerEndpointUri()
        {
            CreateStandardConstructServerEndpointUri("net.pipe", "localhost");
        }
        /// <summary>
        /// <br />Scheme: net.pipe
        /// <br />Hostname: daisy
        /// </summary>
        [TestMethod]
        public void _1_2_S1_CreateStandardConstructServerEndpointUri()
        {
            CreateStandardConstructServerEndpointUri("net.pipe", "daisy");
        }
        /// <summary>
        /// <br />Scheme: net.pipe
        /// <br />Hostname: daisy.colum.edu
        /// </summary>
        [TestMethod]
        public void _1_3_S1_CreateStandardConstructServerEndpointUri()
        {
            CreateStandardConstructServerEndpointUri("net.pipe", "daisy.colum.edu");
        }
        /// <summary>
        /// <br />Scheme: http
        /// <br />Hostname: localhost
        /// <br />Port: 8000
        /// </summary>
        [TestMethod]
        public void _2_1_S1_CreateStandardConstructServerEndpointUri()
        {
            CreateStandardConstructServerEndpointUri("http", "localhost", 8000);
        }
        /// <summary>
        /// <br />Scheme: http
        /// <br />Hostname: daisy
        /// <br />Port: 8000
        /// </summary>
        [TestMethod]
        public void _2_2_S1_CreateStandardConstructServerEndpointUri()
        {
            CreateStandardConstructServerEndpointUri("http", "daisy", 8000);
        }
        /// <summary>
        /// <br />Scheme: http
        /// <br />Hostname: daisy.colum.edu
        /// <br />Port: 8000
        /// </summary>
        [TestMethod]
        public void _2_3_S1_CreateStandardConstructServerEndpointUri()
        {
            CreateStandardConstructServerEndpointUri("http", "daisy.colum.edu", 8000);
        }

    }
}
