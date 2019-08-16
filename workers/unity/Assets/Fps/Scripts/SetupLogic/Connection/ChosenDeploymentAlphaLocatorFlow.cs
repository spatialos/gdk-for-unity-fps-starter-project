using System;
using System.Collections.Generic;
using System.Linq;
using Improbable.Gdk.Core;
using Improbable.Worker.CInterop.Alpha;

namespace Fps.Connection
{
    public class ChosenDeploymentAlphaLocatorFlow : AlphaLocatorFlow
    {
        private readonly string targetDeployment;

        public ChosenDeploymentAlphaLocatorFlow(string targetDeployment,
            IConnectionFlowInitializer<AlphaLocatorFlow> initializer = null) : base(initializer)
        {
            this.targetDeployment = targetDeployment;
        }

        protected override string SelectLoginToken(List<LoginTokenDetails> loginTokens)
        {
            var token = loginTokens.FirstOrDefault(loginToken => loginToken.DeploymentName == targetDeployment);

            return token.LoginToken ?? throw new ArgumentException("Was not able to connect to deployment");
        }
    }
}
