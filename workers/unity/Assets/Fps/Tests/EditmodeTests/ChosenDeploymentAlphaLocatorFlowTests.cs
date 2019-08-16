using System;
using Fps.Connection;
using Improbable.Gdk.TestUtils.Editor;
using Improbable.Gdk.Tools;
using Improbable.Worker.CInterop;
using NUnit.Framework;

namespace Fps.EditmodeTests
{
    [TestFixture]
    public class ChosenDeploymentAlphaLocatorFlowTests
    {
        private const string LaunchConfigName = "default_launch.json";

        private SpatialdManager spatiald;
        private Improbable.Worker.CInterop.Connection connection;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            if (!DevAuthTokenUtils.TryGenerate())
            {
                throw new Exception("Could not generate a dev auth token.");
            }

            spatiald = SpatialdManager.Start().Result;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            spatiald?.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            connection?.Dispose();
            connection = null;
        }

        [Test]
        public void CreateAsync_succeeds_if_there_is_matching_deployment()
        {
            using (var depl = spatiald.StartLocalDeployment("target_deployment", LaunchConfigName).Result)
            {
                depl.AddDevLoginTag().Wait();

                var flow = GetFlow("target_deployment");
                flow.DevAuthToken = DevAuthTokenUtils.DevAuthToken;

                Assert.DoesNotThrow(() => connection = flow.CreateAsync(GetConnectionParameters()).Result);
                Assert.AreEqual(ConnectionStatusCode.Success, connection.GetConnectionStatusCode());
            }
        }

        [Test]
        public void CreateAsync_fails_if_no_matching_deployment()
        {
            using (var depl = spatiald.StartLocalDeployment("a_different_deployment", LaunchConfigName).Result)
            {
                depl.AddDevLoginTag().Wait();

                var flow = GetFlow("target_deployment");
                flow.DevAuthToken = DevAuthTokenUtils.DevAuthToken;

                var aggregateException = Assert.Throws<AggregateException>(() => connection = flow.CreateAsync(GetConnectionParameters()).Result);
                Assert.IsInstanceOf<ArgumentException>(aggregateException.InnerExceptions[0]);
            }
        }

        private static ChosenDeploymentAlphaLocatorFlow GetFlow(string targetDeployment)
        {
            return new ChosenDeploymentAlphaLocatorFlow(targetDeployment)
            {
                LocatorHost = "localhost",
                LocatorPort = 9876,
                UseInsecureConnection = true
            };
        }

        private static ConnectionParameters GetConnectionParameters()
        {
            return new ConnectionParameters
            {
                WorkerType = "UnityClient",
                DefaultComponentVtable = new ComponentVtable()
            };
        }
    }
}
